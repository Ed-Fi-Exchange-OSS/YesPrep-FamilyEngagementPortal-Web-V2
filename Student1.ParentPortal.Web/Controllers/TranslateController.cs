﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Resources.Providers.Translation;
using Student1.ParentPortal.Resources.Services.Translate;

namespace Student1.ParentPortal.Web.Controllers
{
    [RoutePrefix("api/translate")]
    public class TranslateController : ApiController
    {
        private readonly ITranslationProvider _translationProvider;
        private readonly ITranslateService _translateService;

        public TranslateController(ITranslationProvider translationProvider, ITranslateService translateService)
        {
            _translationProvider = translationProvider;
            _translateService = translateService;
        }

        [AllowAnonymous]
        [Route("languages")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAvailableLanguages()
        {
            var model = await _translationProvider.GetAvailableLanguagesAsync();

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [Route("autoDetect")]
        [HttpPost]
        public async Task<IHttpActionResult> TranslateAutoDetect(TranslateRequest request)
        {
            var model = await _translationProvider.AutoDetectTranslateAsync(request);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Translate(TranslateRequest request)
        {
            var model = await _translationProvider.TranslateAsync(request);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [Route("package")]
        [HttpPost]
        public async Task<IHttpActionResult> CreatePackLang(TranslatePackageModelRequest request)
        {
            var baseLangFile = HttpContext.Current.Server.MapPath("~/clientapp/languages/en-us.js");
            var folderFiles = HttpContext.Current.Server.MapPath("~/clientapp/languages/");
            var languageConfigPath = HttpContext.Current.Server.MapPath("~/clientapp/languages/languages.module.js");

            return Ok(await _translateService.CreatePackagesLanguages(request, baseLangFile, folderFiles, languageConfigPath));
        }

        [Route("package/add/element")]
        [HttpPost]
        public async Task<IHttpActionResult> AddElementInPackageLanguage(TranslateElementRequest request)
        {
            var baseLangFile = HttpContext.Current.Server.MapPath("~/clientapp/languages/en-us.js");
            return Ok(_translateService.AddElementToPackageLanguage(request, baseLangFile));
        }

    }
}
