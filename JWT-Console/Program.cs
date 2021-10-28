﻿using System;
using DocuSign.CodeExamples.Authentication;

namespace DocuSign.CodeExamples.JWT_Console
{
    class Program
    {
        static void Main(string[] args)
        {

            string accessToken, accountId, baseUri;
            (accessToken, accountId, baseUri) = JWTAuth.AuthenticateWithJWT();

            Console.WriteLine("Hello World!");
        }
    }
}
