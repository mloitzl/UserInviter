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
                                .Filter("UserState eq 'PendingAcceptance'")
                                .Select("displayName,id,userPrincipalName")
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
            // if (!string.IsNullOrEmpty(newEvent.Email))
            // {
            var successes = new List<IActionResult>();
            var errors = new List<IActionResult>();
            var invitees = newEvent.Email.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var invitee in invitees)
            {
                try
                {
                    var invitation = new Invitation
                    {
                        InvitedUserEmailAddress = invitee,
                        InviteRedirectUrl = newEvent.RedirectUri.ToString(),
                        SendInvitationMessage = true
                    };

                    var invitationResult = await _graphClient.Invitations
                        .Request()
                        .AddAsync(invitation);

                    successes.Add(new RedirectToActionResult("Index", "Invite", new object()).WithSuccess($"User {invitee} invited"));

                    // return RedirectToAction("Index").WithSuccess($"User {invitee} invited");
                }
                catch (ServiceException ex)
                {
                    if (ex.InnerException is MicrosoftIdentityWebChallengeUserException)
                    {
                        throw;
                    }

                    errors.Add(
                        new RedirectToActionResult("Index", "Invite", new object())
                                .WithError("Error creating event", ex.Error.Message));
                }
            }


            if (successes.Any()) return successes.First();
            if (errors.Any()) return errors.First();

            return StatusCode(500);
            // }
        }
    }
}