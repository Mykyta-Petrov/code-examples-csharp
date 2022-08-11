﻿using DocuSign.CodeExamples.Controllers;
using DocuSign.CodeExamples.eSignature.Models;
using DocuSign.CodeExamples.Models;
using DocuSign.CodeExamples.Rooms.Models;
using DocuSign.Rooms.Client;
using DocuSign.Rooms.Examples;
using DocuSign.Rooms.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.CodeExamples.Rooms.Controllers
{
    [Area("Rooms")]
    [Route("Eg09")]
    public class Eg09AssignFormToFormGroupController : EgController
    {
        public Eg09AssignFormToFormGroupController(
            DSConfiguration dsConfig,
            LauncherTexts launcherTexts,
            IRequestItemsService requestItemsService) : base(dsConfig, launcherTexts, requestItemsService)
        {
            codeExampleText = GetExampleText(EgNumber);
            ViewBag.title = codeExampleText.PageTitle;
        }

        public const int EgNumber = 9;

        public override string EgName => "Eg09";

        [BindProperty]
        public FormFormGroupModel FormFormGroupModel { get; set; }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            FormFormGroupModel = new FormFormGroupModel();
        }

        [MustAuthenticate]
        [HttpGet]
        public override IActionResult Get()
        {
            base.Get();

            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to get forms and form groups
                (FormSummaryList forms, FormGroupSummaryList formGroups) =
                    AssignFormToFormGroups.GetFormsAndFormGroups(basePath, accessToken, accountId);

                FormFormGroupModel = new FormFormGroupModel { Forms = forms.Forms, FormGroups = formGroups.FormGroups };

                return View("Eg09", this);
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message;

                return View("Error");
            }
        }

        [MustAuthenticate]
        [Route("AssignFormToFormGroup")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignFormToFormGroup(FormFormGroupModel formFormGroupModel)
        {
            // Obtain your OAuth token
            string accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var basePath = $"{RequestItemsService.Session.RoomsApiBasePath}/restapi"; // Base API path
            string accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            try
            {
                // Call the Rooms API to assign form to form group
                var formGroupFormToAssign = AssignFormToFormGroups.AssignForm(basePath, accessToken, accountId,
                    formFormGroupModel.FormGroupId, new FormGroupFormToAssign() { FormId = formFormGroupModel.FormId });

                ViewBag.h1 = codeExampleText.ResultsPageHeader;
                ViewBag.message = string.Format(
                    codeExampleText.ResultsPageText, 
                    formGroupFormToAssign.FormId, 
                    formFormGroupModel.FormGroupId.ToString());
                ViewBag.Locals.Json = JsonConvert.SerializeObject(formGroupFormToAssign, Formatting.Indented);

                return View("example_done");
            }
            catch (ApiException apiException)
            {
                ViewBag.errorCode = apiException.ErrorCode;
                ViewBag.errorMessage = apiException.Message.Equals("Unhandled response type.") ? 
                    "Response is empty and could not be cast to FormGroupFormToAssign. " +
                    "Please contact DocuSign support" : apiException.Message;

                return View("Error");
            }
        }
    }
}
