using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.SignIn_SignUp
{
    class SignUp
    {
        public async void Verificare()
        {
            
            string token = "dbasyigdfibcakiufhas";
            await SecureStorage.Default.SetAsync("tekenulMeu", token);

        }
    }
}
