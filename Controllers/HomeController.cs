using System.Threading.Tasks;
using Catalog.Models;
using Catalog.Repositories;
using Catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Catalog.Controllers
{ 
    public class Login : ControllerBase
    {
        [HttpPost]
        [Route("login")]
                
        public async Task<ActionResult<dynamic>> Authenticate([FromBody]User model)
        {
            var user = UserRepository.Get(model.Username, model.Password);

               if (user == null)
                 return NotFound();
         
            var token = TokenService.GenerateToken(user);
          
            user.Password = "";
                        
            return new
            {
                user = user,
                token = token
            };
        }
    }
}
