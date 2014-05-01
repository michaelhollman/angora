using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Angora.Services;

namespace Angora.Web.Controllers
{
    public class SimplexController : Controller
    {
        private ISimplexService _simplexService;

        public SimplexController(ISimplexService service)
        {
            _simplexService = service;
        }
        //
        // GET: /Simplex/
        public ActionResult Index()
        {
            _simplexService.PerformSimplex();
            return new EmptyResult();
        }
	}
}