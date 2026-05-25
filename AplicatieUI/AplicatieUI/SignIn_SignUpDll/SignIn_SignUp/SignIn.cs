/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Handles the user sign-in process. It acts as a bridge between the UI and the API service, 
 * responsible for verifying credentials by invoking the backend login methods.
 */


using AplicatieUI.Logica.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.SignIn_SignUp
{
    public class SignIn
    {
        private readonly ApiService _apiService = new ApiService();


        /// <summary>
        /// Verifies user credentials through the API service.
        /// </summary>
        public async Task<ApiResult> Verificare(string email, string password)
        {
            return await _apiService.LoginAsync(email, password);
        }
    }
}
