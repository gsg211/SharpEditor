using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AplicatieUI.Logica.API;

namespace AplicatieUI.Logica.SignIn_SignUp
{
    class SignUp
    {
        private readonly ApiService _apiService = new ApiService();

        public async Task<ApiResult> ExecutaInregistrare(string user, string email, string pass)
        {
            return await _apiService.RegisterAsync(user, email, pass);
        }
    }
}
