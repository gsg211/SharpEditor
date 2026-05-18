using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.SignIn_SignUp
{
    class SignIn
    {
        public async Task<bool> Verificare(string name, string password)
        {
            string numeTemp = "a";
            string par = "a";

            if (name == numeTemp && password == par)
            {
                string token = "dbasyigdfibcakiufhas";
                await SecureStorage.Default.SetAsync("tekenulMeu", token);

                return true;
            }


           return false;
        }
    }
}
