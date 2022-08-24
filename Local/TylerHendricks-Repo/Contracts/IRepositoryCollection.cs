using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Repo.Contracts
{
    public interface IRepositoryCollection: IDisposable
    {
        IPatient Patients { get; }
        IStates States { get; }
        IPhysician Physician { get; }
        IAdmin Admin { get; }
        IAmazonCustom AmazonCustom { get; }
    }
}
