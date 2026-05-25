using AplicatieUI.Logica.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.SignIn_SignUp
{
    class SignIn
    {
        private readonly ApiService _apiService = new ApiService();

        public async Task<ApiResult> Verificare(string email, string password)
        {
            return await _apiService.LoginAsync(email, password);
        }
    }
}
