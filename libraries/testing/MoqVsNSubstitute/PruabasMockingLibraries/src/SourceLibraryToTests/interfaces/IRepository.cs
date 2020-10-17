using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SourceLibraryToTests.interfaces
{
    public interface IRepository : IDisposable
    {
        IList<IUserModel> Users { get; set; }

        IList<IUserModel> ActiveUsers();

        Task<IList<IUserModel>> ActiveUsersAsync();

        IUserModel SearchById(int id);

        IList<IUserModel> Search(string address);

        bool Add(IUserModel model);

        bool AddUser<T>(T user);

        void Save();


    }
}
