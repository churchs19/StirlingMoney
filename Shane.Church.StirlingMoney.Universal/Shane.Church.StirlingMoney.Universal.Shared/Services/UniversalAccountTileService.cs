using Shane.Church.StirlingMoney.Core.Data;
using Shane.Church.StirlingMoney.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shane.Church.StirlingMoney.Universal.Services
{
    public class UniversalAccountTileService : ITileService<Account, Guid>
    {
        public bool TileExists(Guid id)
        {
            //throw new NotImplementedException();
            return false;
        }

        public void AddTile(Guid id)
        {
            //throw new NotImplementedException();
        }

        public void UpdateTile(Guid id)
        {
            //throw new NotImplementedException();
        }

        public void DeleteTile(Guid Id)
        {
            //throw new NotImplementedException();
        }
    }
}
