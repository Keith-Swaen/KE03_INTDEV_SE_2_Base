﻿using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    // Deze class vult de database met testdata bij het opstarten.
    public static class MatrixIncDbInitializer
    {
        public static void Initialize(MatrixIncDbContext context)
        {
            // Check of er al klanten zijn
            // Als er klanten zijn, seed niet opnieuw
            if (context.Customers.Any())
            {
                return;
            }

            // Voeg klanten toe
            var customers = new Customer[]
            {
                new Customer { Name = "Neo", Address = "123 Elm St", Active = true },
                new Customer { Name = "Morpheus", Address = "456 Oak St", Active = true },
                new Customer { Name = "Trinity", Address = "789 Pine St", Active = true }
            };
            context.Customers.AddRange(customers);

            // Voeg producten toe
            var products = new Product[]
            {
                new Product { Name = "Nebuchadnezzar", Description = "Het schip waarop Neo voor het eerst de echte wereld leert kennen", Price = 10000.00m, StockQuantity = 1 },
                new Product { Name = "Jack-in Chair", Description = "Stoel waarin mensen ingeplugd worden in de Matrix", Price = 500.50m, StockQuantity = 10 },
                new Product { Name = "EMP Device", Description = "Wapentuig op de schepen van Zion", Price = 129.99m, StockQuantity = 5 }
            };
            context.Products.AddRange(products);

            // Maak orders aan zonder producten
            var orders = new Order[]
            {
                new Order { Customer = customers[0], OrderDate = DateTime.Parse("2021-01-01"), Status = OrderStatus.InWacht},
                new Order { Customer = customers[0], OrderDate = DateTime.Parse("2021-02-01"), Status = OrderStatus.InBehandeling},
                new Order { Customer = customers[1], OrderDate = DateTime.Parse("2021-02-01"), Status = OrderStatus.Geannuleerd},
                new Order { Customer = customers[2], OrderDate = DateTime.Parse("2021-03-01"), Status = OrderStatus.Verzonden}
            };
            context.Orders.AddRange(orders);
            // Sla op zodat de Id's bekend zijn
            context.SaveChanges();

            // Voeg order product koppelingen toe
            var orderProducts = new List<OrderProduct>
            {
                new OrderProduct { Order = orders[0], Product = products[0], ProductName = products[0].Name, ProductPrice = products[0].Price, Quantity = 1 },
                new OrderProduct { Order = orders[0], Product = products[1], ProductName = products[1].Name, ProductPrice = products[1].Price, Quantity = 1 },
                new OrderProduct { Order = orders[1], Product = products[2], ProductName = products[2].Name, ProductPrice = products[2].Price, Quantity = 1 },
                new OrderProduct { Order = orders[2], Product = products[0], ProductName = products[0].Name, ProductPrice = products[0].Price, Quantity = 1 },
                new OrderProduct { Order = orders[2], Product = products[2], ProductName = products[2].Name, ProductPrice = products[2].Price, Quantity = 1 },
                new OrderProduct { Order = orders[3], Product = products[1], ProductName = products[1].Name, ProductPrice = products[1].Price, Quantity = 1 }
            };
            context.AddRange(orderProducts);

            // Voeg onderdelen toe
            var parts = new Part[]
            {
                new Part { Name = "Tandwiel", Description = "Overdracht van rotatie in bijvoorbeeld de motor of luikmechanismen" },
                new Part { Name = "M5 Boutje", Description = "Bevestiging van panelen, buizen of interne modules" },
                new Part { Name = "Hydraulische cilinder", Description = "Openen/sluiten van zware luchtsluizen of bewegende onderdelen" },
                new Part { Name = "Koelvloeistofpomp", Description = "Koeling van de motor of elektronische systemen." }
            };
            context.Parts.AddRange(parts);

            // Voeg admin toe
            var admins = new Admin[]
            {
                new Admin { Username = "admin", Password = "matrix123" }
            };
            context.Admins.AddRange(admins);

            // Sla alles op
            context.SaveChanges();
            // Zorg dat de database bestaat
            context.Database.EnsureCreated();
        }
    }
}