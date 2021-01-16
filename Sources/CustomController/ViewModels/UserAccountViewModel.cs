using CustomController.Helpers;

namespace CustomController
{
    public class UserAccountViewModel
    {
        public string Token { get; set; }
        public string Company { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void ParseToken()
        {
            var decodedToken = Util.Base64Decode(this.Token);
            var splitted = decodedToken.Split(':');
            if (splitted.Length < 2)
                return;
            else
            {
                if (splitted.Length > 2)
                {
                    this.Company = splitted[0].ToUpper();
                    this.Username = splitted[1].ToUpper();
                    this.Password = splitted[2];
                }
                else if (splitted.Length == 2)
                {
                    this.Username = splitted[0].ToUpper();
                    this.Password = splitted[1];
                }
            }
        }
    }
}
