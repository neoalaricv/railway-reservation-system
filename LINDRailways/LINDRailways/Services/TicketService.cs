﻿using LINDRailways.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LINDRailways.Services
{
    public static class TicketService
    {
        private static SQLiteAsyncConnection Database;
        private static async Task Init()
        {
            if (Database is not null)
                return;

            string databaseFilename = "Tickets.db3";
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseFilename);

            SQLite.SQLiteOpenFlags flags =
                SQLite.SQLiteOpenFlags.ReadWrite |
                SQLite.SQLiteOpenFlags.Create |
                SQLite.SQLiteOpenFlags.SharedCache;

            Database = new SQLiteAsyncConnection(databasePath, flags);

            var result = await Database.CreateTableAsync<Ticket>();
        }

        public static async Task AddTicket(string passengerName, int isMale,
            int isPaid, string departureDate, string trainName, string origin,
            string destination, string departureTime)
        {
            await Init();

            var ticket = new Ticket
            {
                PassengerName = passengerName,
                IsMale = isMale,
                IsPaid = isPaid,
                DepartureDate = departureDate,
                TrainName = trainName,
                Origin = origin,
                Destination = destination,
                DepartureTime = departureTime
            };

            await Database.InsertAsync(ticket);
        }

        public static async Task RemoveTicket(int id)
        {
            await Init();

            await Database.DeleteAsync<Ticket>(id);
        }

        public static async Task PayTicket(Ticket ticket)
        {
            await Init();

            var newTicket = new Ticket
            {
                PassengerName = ticket.PassengerName,
                IsMale = ticket.IsMale,
                IsPaid = 1,
                DepartureDate = ticket.DepartureDate,
                TrainName = ticket.TrainName,
                Origin = ticket.Origin,
                Destination = ticket.Destination,
                DepartureTime = ticket.DepartureTime
            };

            await Database.InsertAsync(newTicket);

            await Database.DeleteAsync<Ticket>(ticket.Id);
        }

        public static async Task<IEnumerable<Ticket>> GetAllTickets()
        {
            await Init();

            var ticket = await Database.Table<Ticket>().ToListAsync();

            return ticket;
        }
    }
}
