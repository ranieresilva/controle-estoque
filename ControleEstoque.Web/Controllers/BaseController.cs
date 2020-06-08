using AutoMapper;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    public class BaseController : Controller
    {
        public IMapper Mapper
        {
            get
            {
                var ret = (HttpContext.Items["Mapper"] as IMapper);
                return ret;
            }
        }
    }
}