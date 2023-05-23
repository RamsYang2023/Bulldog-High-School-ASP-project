using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BullDoghs.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

using aBCryptNet = BCrypt.Net.BCrypt;

namespace BullDoghs.Controllers
{
    public class AccountController : Controller
    {
        

        private readonly Team105DBContext _team105DBContext;

        public AccountController(Team105DBContext courseContext)
        {
            _team105DBContext = courseContext;
        }

        // the returnURL captures the View the user was trying to reach before being redirected to the Login View
        
        
        public IActionResult Login(string returnURL)
        {
            // if returnURL is null or empty, it is set to "/" (i.e., Home/Index)

            returnURL = String.IsNullOrEmpty(returnURL) ? "~/" : returnURL;

            // create a new instance of LoginInput and pass it to the Login View

            return View(new LoginInput { ReturnURL = returnURL });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,UserPassword,ReturnURL")] LoginInput loginInput)
        {
            if (ModelState.IsValid)
            {
                // retrieve user information

                var aUser = await _team105DBContext.LoginInfos.FirstOrDefaultAsync(u => u.UserName == loginInput.UserName);

                // if user exists and passwords match

                if (aUser != null && aBCryptNet.Verify(loginInput.UserPassword, aUser.UserPass))
                {
                    // From Microsoft documentation - "A claim is a statement about a subject by an issuer. Claims represent attributes of the subject that are useful in the context of authentication and authorization operations"

                    // Examples of claims would be data on a Driver's License card (i.e., name, date of birth)

                    var claims = new List<Claim>();

                    // the Type property can be used to store information about the claim

                    claims.Add(new Claim(ClaimTypes.Name, aUser.FullName));
                    claims.Add(new Claim(ClaimTypes.Sid, aUser.UserPK.ToString()));

                    // role(s) are stored as a comma-delimited list in the "UserRoles" column in the LoginInfo table

                    //string[] roles = aUser.UserRole.Split(",");
                    string role = aUser.UserRole;

                        claims.Add(new Claim(ClaimTypes.Role, role));


                    // From Microsoft documentation - "The ClaimsIdentity class is a concrete implementation of a claims-based identity; that is, an identity described by a collection of claims."

                    // a collection of claims can be used to create a ClaimsIndentity along with the authentication scheme (in this case, cookie-based authentication)

                    // Example of identity would be a Driver's License card
                    
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // multiple identities can be stored in a ClaimsPrincipal

                    // Example, a user's multiple identities (driver's license, employee ID, passport) can make up a ClaimsPrincipal

                    var principal = new ClaimsPrincipal(identity);

                    // the SignInAsync method issues the authentication cookie to the user

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // return the user to the View they were originally trying to reach or Home/Index

                    return Redirect(loginInput?.ReturnURL ?? "~/");

                }

                // if credentials are not valid

                else
                {
                    ViewData["message"] = "Invalid credentials";
                }
            }

            // return user to Login View

            return View(loginInput);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        // Post action (when user submits the SignUp form)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("UserName,UserPass,FullName,UserRole")] LoginInfo loginInfo)
        {
            if (ModelState.IsValid)
            {
                // check for duplicate username

                var aUser = await _team105DBContext.LoginInfos.FirstOrDefaultAsync(u => u.UserName == loginInfo.UserName);
                var aStudentTest = await _team105DBContext.tbStudents.FirstOrDefaultAsync(u => u.Email == loginInfo.UserName); 
                var aTeacherTest = await _team105DBContext.tbTeachers.FirstOrDefaultAsync(u => u.Email == loginInfo.UserName);
                // if no duplication

                if (aUser is null && (aStudentTest  is not null || aTeacherTest is not null))
                {
                    // hash password

                    loginInfo.UserPass = aBCryptNet.HashPassword(loginInfo.UserPass);

                    // set default role to "user" and create new record in LoginInfo
                    
                    
                    if (aStudentTest is not null) { loginInfo.UserRole = "Student"; }

                    
                    if (aTeacherTest is not null) { loginInfo.UserRole = "Teacher"; }

                   
                    //loginInfo.UserRole = "Student";

                    _team105DBContext.Add(loginInfo);
                    await _team105DBContext.SaveChangesAsync();

                    TempData["success"] = "Account created";

                    // redirect to Login View

                    return RedirectToAction(nameof(Login));
                }
                else if(aStudentTest is null && aTeacherTest is null)
                {
                     ViewData["message"] = "Invalid User at Bulldog High School!"; 
                }
                else
                {
                    ViewData["message"] = "Choose a different username";
                }
            }
            // return user to SignUp View

            return View(loginInfo);
        }

        // method to log user out and redirect to Home View
        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
       
    }
}
