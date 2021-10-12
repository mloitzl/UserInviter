using System;
using System.ComponentModel.DataAnnotations;

namespace com.loitzl.userinviter.Models
{
    public class NewInviteViewModel
    {
        public Uri RedirectUri { get; set; }
        // todo: single only?

        [RegularExpression(@"((\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)*([;])*)*",
         ErrorMessage = "Please enter one or more email addresses separated by a semi-colon (;)")]
        public string Email { get; set; }
    }
}