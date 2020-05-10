using CypherK_Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CypherK_BlazorApp.Data.ViewModels
{
    public class CypherViewModel
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public string Key { get; set; }
    }



    public class EncryptViewModel
    {
        [Required]
        public string TextToEncrypt { get; set; }
        public string CypherKey { get; set; }
        public string AuthorEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string SelectedAlgorithm { get; set; }

        public Dictionary<string, string> Algorithms
        {
            get
            {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                ret.Add(Algorithm.bacon.ToString(), "Szyfr Bacona" );
                ret.Add(Algorithm.cezar.ToString(), "Szyfr Cezara" );
                ret.Add(Algorithm.aes.ToString(), "AES SymmetricAlgorithm" );
                return ret;
            }
        }
    }



    public class DecryptViewModel
    {
        [Required]
        public string TextToDecrypt { get; set; }
        public string CypherKey { get; set; }
        public string SelectedAlgorithm { get; set; }
        public string ReceiverEmail { get; set; }

        public Dictionary<string, string> Algorithms
        {
            get
            {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                ret.Add(Algorithm.undefined.ToString(), "Nie znam algorytmu szyfrowania");
                ret.Add(Algorithm.bacon.ToString(), "Szyfr Bacona");
                ret.Add(Algorithm.cezar.ToString(), "Szyfr Cezara");
                ret.Add(Algorithm.aes.ToString(), "AES SymmetricAlgorithm");
                return ret;
            }
        }
    }


}
