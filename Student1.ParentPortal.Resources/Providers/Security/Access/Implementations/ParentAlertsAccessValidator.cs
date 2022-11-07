﻿using System;

namespace Student1.ParentPortal.Resources.Providers.Security.Access
{
    public class ParentAlertsAccessValidator : IRoleResourceAccessValidator
    {
        public bool CanHandle(SecurityContext securityContext)
        {
            return securityContext.UserRoleAccessingResource.Equals("parent", StringComparison.InvariantCultureIgnoreCase)
                && securityContext.ResourceName.Equals("alerts", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanAccess(SecurityContext securityContext)
        {
            // As of now this validator returns true because:
            // The simplistic design of this application does not require further complexity
            // All data requests to get alerts have primary relationship with the parent who requested.
            // There is no implementation or requirement for parents looking at other parents.
            // A parentUSI can see its own role based on the current principal.
            // There is no parameter to validate.
            return true;
        }
    }
}
