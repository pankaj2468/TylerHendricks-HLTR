using System;
using TylerHendricks_Repo.Contracts;

namespace TylerHendricks_Repo.Services
{
    public class RepositoryCollection : IRepositoryCollection
    {
        public IPatient Patients 
        {
            get { return this.Patients = new Patient(); }
            private set { }
        }
        public IStates States
        {
            get { return this.States = new States(); }
            private set { }
        }
        public IPhysician Physician
        {
            get { return this.Physician = new Physician(); }
            private set { }
        }
        public IAdmin Admin
        {
            get { return this.Admin = new Admin(); }
            private set { }
        }
        public IAmazonCustom AmazonCustom
        {
            get { return this.AmazonCustom = new AmazonCustom(); }
            private set { }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //free managed resources
                if (Patients != null)
                {
                    Patients.Dispose();
                    Patients = null;
                }
                //free managed resources
                if (States!=null)
                {
                    States.Dispose();
                    States = null;
                }
                //free managed resources
                if (Physician != null)
                {
                    Physician.Dispose();
                    Physician = null;
                }
                //free managed resources
                if (Admin != null)
                {
                    Admin.Dispose();
                    Admin = null;
                }
                //free managed resources
                if (AmazonCustom != null)
                {
                    AmazonCustom.Dispose();
                    AmazonCustom = null;
                }
            }
        }
    }
}
