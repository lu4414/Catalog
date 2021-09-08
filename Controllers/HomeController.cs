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
            // Recupera o usuário
            var user = UserRepository.Get(model.Username, model.Password);

             //Verifica se o usuário existe
               if (user == null)
                 return NotFound();

            // Gera o Token
            var token = TokenService.GenerateToken(user);

            // Oculta a senha
            user.Password = "";
            
            // Retorna os dados
            return new
            {
                user = user,
                token = token
            };
        }
    }
}
