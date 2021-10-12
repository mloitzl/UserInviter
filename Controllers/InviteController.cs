using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.loitzl.userinviter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace com.loitzl.userinviter.Controllers
{
    public class InviteController : Controller
    {
        private readonly GraphServiceClient _graphClient;
        private readonly ILogger<InviteController> _logger;

        public InviteController(
            GraphServiceClient graphClient,
            ILogger<InviteController> logger)
        {
            _graphClient = graphClient;
            _logger = logger;
        }

        private async Task<IList<User>> GetUsers()
        {

            var users = await _graphClient
                                .Users
                                .Request()
                                // .Filter("identities/any(c:c/issuerAssignedId eq 'j.smith@yahoo.com' and c/issuer eq 'contoso.onmicrosoft.com')")
                                .Select("displayName,id")
                                .GetAsync();

            return users.CurrentPage;
        }

        // Minimum permission scope needed for this view
        [AuthorizeForScopes(Scopes = new[] { "User.ReadWrite.All" })]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await GetUsers();

                var model = users.Select(user => new UserViewModel(user)).ToList();

                return View(model);
            }
            catch (ServiceException ex)
            {
                if (ex.InnerException is MicrosoftIdentityWebChallengeUserException)
                {
                    throw;
                }

                return new ContentResult
                {
                    Content = $"Error getting calendar view: {ex.Message}",
                    ContentType = "text/plain"
                };
            }
        }

        // Minimum permission scope needed for this view
        [AuthorizeForScopes(Scopes = new[] { "User.Invite.All" })]
        public IActionResult New()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeForScopes(Scopes = new[] { "User.Invite.All" })]
        public async Task<IActionResult> New([Bind("RedirectUri,Email")] NewInviteViewModel newEvent)
        {
            if (!string.IsNullOrEmpty(newEvent.Email))
            {
                var attendees =
                    newEvent.Email.Split(';', StringSplitOptions.RemoveEmptyEntries);

                if (attendees.Length > 0)
                {
                }
            }


            // todo: foreach user
            try
            {
                var invitation = new Invitation
                {
                    InvitedUserEmailAddress = "",
                    InviteRedirectUrl = "https://myapp.contoso.com"
                };

                await _graphClient.Invitations
                    .Request()
                    .AddAsync(invitation);

                // Redirect to the calendar view with a success message
                return RedirectToAction("Index").WithSuccess("Event created");
            }
            catch (ServiceException ex)
            {
                // Redirect to the calendar view with an error message
                return RedirectToAction("Index")
                    .WithError("Error creating event", ex.Error.Message);
            }
        }
    }
}