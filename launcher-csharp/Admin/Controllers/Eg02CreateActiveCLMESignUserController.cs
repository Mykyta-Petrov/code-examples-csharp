﻿using DocuSign.Admin.Examples;
using DocuSign.OrgAdmin.Api;
using DocuSign.OrgAdmin.Client;
using DocuSign.CodeExamples.Common;
using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DocuSign.CodeExamples.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/Eg02")]
    public class Eg02CreateActiveCLMESignUserController : EgController
    {
        private static Guid? clmProductId;
        private static Guid? eSignProductId;
        public Eg02CreateActiveCLMESignUserController(
            DSConfiguration dsConfig,
            IRequestItemsService requestItemsService)
            : base(dsConfig, requestItemsService)
        {
        }

        public override string EgName => "Eg02";

        protected override void InitializeInternal()
        {
            var organizationId = RequestItemsService.OrganizationId;
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = "https://api-d.docusign.net/management"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;

            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Step 3 Start
            var productPermissionProfileApi = new ProductPermissionProfilesApi(apiClient);
            var productPermissionProfiles = productPermissionProfileApi.GetProductPermissionProfiles(organizationId, Guid.Parse(accountId));
            ViewBag.CLMPermissionProfiles = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "CLM").PermissionProfiles;
            ViewBag.ESignPermissionProfiles = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "ESign").PermissionProfiles;
            clmProductId = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "CLM").ProductId;
            eSignProductId = productPermissionProfiles.ProductPermissionProfiles.Find(x => x.ProductName == "ESign").ProductId;
            // Step 3 End

            // Step 4 Start
            var dsGroupsApi = new DSGroupsApi(apiClient);
            ViewBag.DsGroups = dsGroupsApi.GetDSGroups(organizationId, Guid.Parse(accountId)).DsGroups;
            // Step 4 End
        }

        [MustAuthenticate]
        [Route("Activate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string userName, string firstName, string lastName, string email, string cLMPermissionProfileId, string eSignPermissionProfileId, string dsGroupId)
        {
            // Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken;
            var basePath = $"{RequestItemsService.Session.BasePath}/clickapi"; // Base API path
            var accountId = RequestItemsService.Session.AccountId;
            var orgId = RequestItemsService.OrganizationId;

            try
            {
                var userId = CreateCLMESignUser.Create(userName, firstName, lastName, email, cLMPermissionProfileId, eSignPermissionProfileId, Guid.Parse(dsGroupId), clmProductId, eSignProductId, basePath, accessToken, Guid.Parse(accountId), orgId);
                return View("example_done");           
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }
    }
}