using System.Collections.ObjectModel;
using WebApplication2.Models;

namespace WebApplication2
{
    public class List
    {
        public static Collection<Passanger> Passangers = new Collection<Passanger>()  {
            new Passanger(){Email = "omarabdelgilil@hotmail.com",UserName = "omar abdelgilil",Gender =1},
            new Passanger(){Email = "ibrahim@hotmail.com",UserName = "ibraim abdelgilil",Gender =1},
        };

        public static Collection<Credentials> CredentialsList = new Collection<Credentials>();


    }
}
