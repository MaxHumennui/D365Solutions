using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using DataGenUtil.Enums;

namespace DataGenUtil
{
    class Generator
    {
        static string connectionString = @"AuthType=OAuth;
            Username=;
            Password=;
            Url=https://org7aa364d7.crm.dynamics.com;
            AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
            RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;";

        private CrmServiceClient service = new CrmServiceClient(connectionString);

        public void generateRents(int count)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            cr59f_carclass[] carClasses = getCarClasses();

            for (int i = 0; i < count; i++)
            {
                cr59f_rent rent = new cr59f_rent();

                rent.cr59f_reservedPickup = randomDate(rnd);
                rent.cr59f_reservedHandover = ((DateTime)rent.cr59f_reservedPickup).AddDays(rnd.Next(1, 31));
                rent.cr59f_carClass = (carClasses[rnd.Next(carClasses.Length)]).ToEntityReference();
                rent.cr59f_car = getCar(rent.cr59f_carClass, rnd).ToEntityReference();
                rent.cr59f_customer = getCustomer(rnd).ToEntityReference();
                rent.cr59f_pickupLocation = new OptionSetValue(rnd.Next((int)StatusCode.Created, (int)StatusCode.Renting));
                rent.cr59f_actualPickup = ((DateTime)rent.cr59f_reservedHandover).AddDays(rnd.Next(15));
                rent.statuscode = new OptionSetValue(randomStatusCode(rnd));
                rent.cr59f_price = new Money(rnd.Next(1000, 50000));
                rent.cr59f_paid = true;

                if (rent.statuscode.Value == (int)StatusCode.Confirmed) 
                {
                    if (rnd.Next(101) <= 10)
                        rent.cr59f_paid = false;
                }

                if (rent.statuscode.Value == (int)StatusCode.Renting)
                {
                    createPickupReport(ref rent, rnd);

                    if (rnd.Next(1001) == 1)
                        rent.cr59f_paid = false;
                }

                if (rent.statuscode.Value == (int)StatusCode.Returned)
                {
                    rent.cr59f_actualReturn = ((DateTime)rent.cr59f_actualPickup).AddDays(rnd.Next(15));
                    rent.cr59f_returnLocation = new OptionSetValue(rnd.Next((int)StatusCode.Created, (int)StatusCode.Renting));
                    createPickupReport(ref rent, rnd);
                    createReturnReport(ref rent, rnd);

                    if (rnd.Next(10001) <= 2)
                        rent.cr59f_paid = false;
                }

                if (rent.statuscode.Value > (int)StatusCode.Renting)
                    rent.statecode = (cr59f_rentState?)1;

                service.Create(rent);
                Console.WriteLine(i);
            }
        }

        private DateTime randomDate(Random rnd)
        {
            DateTime start = new DateTime(2019, 1, 1);
            DateTime end = new DateTime(2020, 12, 1);

            int range = (end - start).Days;

            DateTime result = start.AddDays(rnd.Next(range + 1));
            result = result.AddHours(rnd.Next(25));

            return result;
        }

        private cr59f_carclass[] getCarClasses()
        {
            svcContext context = new svcContext(service);

            var carClass = from a in context.cr59f_carclassSet
                           select a;

            var car = from a in context.cr59f_carSet
                      select a;

            cr59f_carclass[] result = new cr59f_carclass[carClass.ToList().Count()];

            int i = 0;
            foreach (var record in carClass)
            {
                foreach (var carsRecords in car)
                {
                    if (record.cr59f_carclass_car_carClass == carsRecords.cr59f_carclass_car_carClass)
                        result[i] = record;
                }

                i++;
            }

            return result;
        }

        private cr59f_car getCar(EntityReference carClass, Random rnd)
        {
            svcContext context = new svcContext(service);

            var car = from a in context.cr59f_carSet
                      where a.cr59f_carClass == carClass
                      select a;

            cr59f_car[] result = new cr59f_car[car.ToList().Count()];

            int i = 0;
            foreach (var record in car)
            {
                result[i] = record;
                i++;
            }

            return result[rnd.Next(result.Length)];
        }

        private Contact getCustomer(Random rnd)
        {
            svcContext context = new svcContext(service);

            var customer = from a in context.ContactSet
                           select a;

            Contact[] result = new Contact[customer.ToList().Count()];

            int i = 0;
            foreach (var record in customer)
            {
                result[i] = record;
                i++;
            }

            return result[rnd.Next(result.Length)];
        }

        private int randomStatusCode(Random rnd)
        {
            int probability = rnd.Next(101);

            if (probability <= 5)
                return (int)StatusCode.Created;
            if (probability > 5 && probability <= 10)
                return (int)StatusCode.Confirmed;
            if (probability > 10 && probability <= 15)
                return (int)StatusCode.Renting;
            if (probability > 15 && probability <= 90)
                return (int)StatusCode.Returned;
            if (probability > 90 && probability <= 100)
                return (int)StatusCode.Canceled;

            return 0;
        }

        private void createPickupReport(ref cr59f_rent rent, Random rnd)
        {
            cr59f_carTransferReport report = new cr59f_carTransferReport();

            report.cr59f_car = rent.cr59f_car;
            report.cr59f_type = new OptionSetValue((int)StatusCode.Created);
            report.cr59f_date = (DateTime)rent.cr59f_actualPickup;

            int probability = rnd.Next(101);
            if (probability <= 5)
            {
                report.cr59f_damages = true;
                report.cr59f_damageDescription = "damage";
            }
            else
            {
                report.cr59f_damages = false;
            }

            service.Create(report);

            svcContext context = new svcContext(service);

            var doneReport = from a in context.cr59f_carTransferReportSet
                             where a.cr59f_date == report.cr59f_date
                             where a.cr59f_type == report.cr59f_type
                             where a.cr59f_car == report.cr59f_car
                             where a.cr59f_damages == report.cr59f_damages
                             select a;

            report = doneReport.First();

            rent.cr59f_pickupReport = report.ToEntityReference();
        }

        private void createReturnReport(ref cr59f_rent rent, Random rnd)
        {
            svcContext context = new svcContext(service);

            Guid pickupReportName = rent.cr59f_pickupReport.Id;

            var rep = from a in context.cr59f_carTransferReportSet
                      where a.cr59f_carTransferReportId == pickupReportName
                      select a;

            cr59f_carTransferReport report = new cr59f_carTransferReport();
            cr59f_carTransferReport pickupReport = rep.First();

            report.cr59f_car = pickupReport.cr59f_car;
            report.cr59f_type = new OptionSetValue((int)StatusCode.Confirmed);
            report.cr59f_date = (DateTime)rent.cr59f_actualReturn;
            report.cr59f_damages = pickupReport.cr59f_damages;
            report.cr59f_damageDescription = pickupReport.cr59f_damageDescription;

            service.Create(report);

            var doneReport = from a in context.cr59f_carTransferReportSet
                             where a.cr59f_date == report.cr59f_date
                             where a.cr59f_type == report.cr59f_type
                             where a.cr59f_car == report.cr59f_car
                             where a.cr59f_damages == report.cr59f_damages
                             select a;

            report = doneReport.First();

            rent.cr59f_returnReport = report.ToEntityReference();
        }

    }
}
