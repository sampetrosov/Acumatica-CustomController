using Newtonsoft.Json.Linq;
using PX.Data;
using PX.Export.Authentication;
using PX.Objects.IN;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using RazorEngine.Templating;
using System.IO;
using RazorEngine;
using CustomController.ViewModels;
using CustomController.Models;
using CustomController.Helpers;
using System.Web;

namespace CustomController
{
    [RoutePrefix("api/v1/IIG")]
    [BasicAuthorize]
    public partial class IIGController : ApiController
    {
        [HttpPost]
        [Route("PostData")]
        public IHttpActionResult PostData([FromBody]JObject data)
        {
            var lead = data.ToObject<BusAccount>();
            if (lead == null)
                return BadRequest("Passed object structure is not corresponding to the requirements");
            var retVal = BusAccount.CreateBusAccount(lead);
            if (retVal)
                return Ok();
            else
                return BadRequest("Error has been thrown while processing the data");
        }
        [HttpGet]
        [Route("Index")]
        [AllowAnonymous]
        public HttpResponseMessage Index()
        {
            var response = new HttpResponseMessage();
            var template = File.ReadAllText(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Frames/IIG/SignIn.cshtml"));
            var layoutTemplate = File.ReadAllText(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Frames/IIG/__Layout.cshtml"));
            Engine.Razor.AddTemplate("__Layout", layoutTemplate);
            var content = Engine.Razor.RunCompile(template, "IIGSignIn");
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        [HttpPost]
        [Route("Index")]
        [AllowAnonymous]
        public HttpResponseMessage Index(UserAccountViewModel viewModel)
        {
            string content = "Not Authorized!";
            if (TryAuthorize(viewModel))
            {
                INSiteMaint siteMaint = PXGraph.CreateInstance<INSiteMaint>();
                var sites = siteMaint.site.Select().Select(pxResult => new INSiteViewModel { Description = pxResult.Record.Descr, SiteCD = pxResult.Record.SiteCD }).ToList();
                var template = File.ReadAllText(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Frames/IIG/Warehouses/Index.cshtml"));
                var model = new { Sites = sites };
                content = Engine.Razor.RunCompile(template, "IIGINSiteIndex", null, model);
            }

            var response = new HttpResponseMessage();
            response.Content = new StringContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private bool TryAuthorize(UserAccountViewModel viewModel)
        {
            viewModel.ParseToken();
            var userName = PXLogin.ConcatLogin(viewModel.Username, viewModel.Company);
            var retVal = PXLogin.LoginUser(ref userName, viewModel.Password);
            if (retVal)
            {
                return true;
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                PXLogin.LogoutUser(PXAccess.GetUserName(), HttpContext.Current.Session.SessionID);
            }
            FormsAuthenticationModule.SignOut();
            base.Dispose(disposing);
        }
    }
}
