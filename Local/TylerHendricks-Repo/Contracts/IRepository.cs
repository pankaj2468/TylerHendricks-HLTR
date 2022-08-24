using System;
using System.Collections.Generic;
using System.Text;
using TylerHendricks_Data.DBEntity;

namespace TylerHendricks_Repo.Contracts
{
   public interface IRepository
    {
        public THContext DBEntity { get; }
    }
}
