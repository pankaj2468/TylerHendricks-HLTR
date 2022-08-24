using System;
using System.Collections.Generic;
using System.Text;
using TylerHendricks_Data.DBEntity;
using TylerHendricks_Repo.Contracts;

namespace TylerHendricks_Repo.Services
{
    public class Repository : IRepository
    {
        public Repository()
        {
            this.DBEntity = new THContext();
        }
        public THContext DBEntity { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize("");
        }
    }
}
