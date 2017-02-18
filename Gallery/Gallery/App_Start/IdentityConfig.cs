using Microsoft.AspNet.Identity;
using Gallery.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;

namespace Gallery
{
    public class IdentityConfig
    {
        public class ApplicationUserStore : UserStore<GalleryUser>
        {
            public ApplicationUserStore(GalleryContext context) : base(context)
            {
            }
        }

        public class ApplicationUserManager : UserManager<GalleryUser>
        {
            public ApplicationUserManager(IUserStore<GalleryUser> store) : base(store)
            {
            }

            public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
            {
                var manager = new ApplicationUserManager(new UserStore<GalleryUser>(context.Get<GalleryContext>()));
                return manager;
            }
        }

        public class ApplicationRoleStore : RoleStore<IdentityRole>
        {
            public ApplicationRoleStore(GalleryContext context) : base(context)
            {
            }
        }

        public class ApplicationRoleManager : RoleManager<IdentityRole>
        {
            public ApplicationRoleManager(IRoleStore<IdentityRole, string> store) : base(store)
            {
            }
            public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
            {
                var roleStore = new RoleStore<IdentityRole>(context.Get<GalleryContext>());
                return new ApplicationRoleManager(roleStore);
            }
        }

        public class ApplicationSignInManager : SignInManager<GalleryUser, string>
        {
            public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
            {
            }


            public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
            {
                return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
            }
        }

    }
}