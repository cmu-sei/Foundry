/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Stack.Data.Transactions
{
    /// <summary>
    /// unit of work transaction manager
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        readonly DbContext _context;
        bool _disposed;
        IUnitOfWorkFlow _flow;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(DbContext context, bool isInMemory = false)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            if (isInMemory)
            {
                _flow = new InMemoryFlow(_context);
            }
            else
            {
                _flow = new AtomicFlow(_context);
            }
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <typeparamref name="DbContext"/>.</returns>
        public DbContext DbContext => _context;

        /// <summary>
        /// Commits the transaction to the database
        /// </summary>
        /// <returns></returns>
        public async Task CommitTransactionAsync()
        {
            await _flow.CommitTransactionAsync();
        }

        public void CommitTransaction()
        {
            _flow.CommitTransactionAsync().Wait();
        }

        /// <summary>
        /// Disposes the unit of work and rolls back uncommitted transactions if no commit took place
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException("UnitOfWork already disposed.");

            _flow.Dispose();

            _disposed = true;
        }
    }
}
