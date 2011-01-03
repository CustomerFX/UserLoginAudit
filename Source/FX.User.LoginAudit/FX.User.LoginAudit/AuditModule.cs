using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sage.Platform.Application;
using Sage.Platform.Security;
using Sage.SalesLogix.Security;
using Sage.Platform;
using Sage.Entity.Interfaces;
using Sage.Platform.Repository;

namespace FX.User.LoginAudit
{
    public class AuditModule : IModule
    {
        public void Load()
        {
            SLXUserService service = (SLXUserService)ApplicationContext.Current.Services.Get<IUserService>();
            RepositoryHelper<IUserLoginAudit> repositoryHelper = EntityFactory.GetRepositoryHelper<IUserLoginAudit>();
            ICriteria criteria = repositoryHelper.CreateCriteria();
            criteria.Add(repositoryHelper.EF.Eq("USERID", service.UserId.Trim()));
            IList<IUserLoginAudit> list = criteria.List<IUserLoginAudit>();
            if ((list != null) && (list.Count > 0))
            {
                foreach (IUserLoginAudit audit in list)
                {
                    if (audit.Login1.HasValue && audit.Login1.Value.Date != DateTime.UtcNow.Date)
                    {
                        audit.Login5=audit.Login4;
                        audit.Login4=audit.Login3;
                        audit.Login3=audit.Login2;
                        audit.Login2=audit.Login1;
                        audit.Login1=DateTime.UtcNow;                        
                        audit.Save();
                    }
                    break;
                }
            }
            else
            {
                IUserLoginAudit audit2 = EntityFactory.Create<IUserLoginAudit>();                                
                audit2.Login1=(new DateTime?(DateTime.UtcNow));                
                audit2.USERID=service.UserId.Trim();
                audit2.Save();
            }
        }

 


    }
}
