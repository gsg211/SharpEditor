/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Manages the user registration logic. 
 * It acts as an intermediary between the UI and the API service, 
 * passing account details to the backend for new user creation.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AplicatieUI.Logica.API;

namespace AplicatieUI.Logica.SignIn_SignUp
{
    public class SignUp
    {
        private readonly ApiService _apiService = new ApiService();



        /// <summary>
        /// Registers a new user account through the API service.
        /// </summary>
        public async Task<ApiResult> ExecutaInregistrare(string user, string email, string pass)
        {
            return await _apiService.RegisterAsync(user, email, pass);
        }
    }
}
