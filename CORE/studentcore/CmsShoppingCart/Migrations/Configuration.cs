namespace CmsShoppingCart.Migrations
{
    using CmsShoppingCart.Models.Data;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CmsShoppingCart.Models.Data.Db>
    {
        public Configuration()
        {
            //AutomaticMigrationDataLossAllowed = true;
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(CmsShoppingCart.Models.Data.Db context)
        {
            //SEED DATA//


            using (Db db = new Db())
            {
                if (db.Roles.Any(x => x.Name.Equals("Admin")))
                {

                }
                else
                {

                    Role Role1 = new Role()

                    {

                        Name = ("Admin"),

                    };

                    Role Role2 = new Role()

                    {
                        Name = ("User")

                    };

                    Role Role3 = new Role()

                    {
                        Name = ("Teacher")

                    };

                    Role Role4 = new Role()

                    {
                        Name = ("Learner")

                    };


                    db.Roles.Add(Role1);
                    db.Roles.Add(Role2);
                    db.Roles.Add(Role3);
                    db.Roles.Add(Role4);

                    db.SaveChanges();

                    UserRole userRoles = new UserRole()

                    {
                        RoleId = 1

                    };

                    db.UserRoles.Add(userRoles);
                    db.SaveChanges();
                    int id = userRoles.UserId;

                    User userDTO = new User()
                    {
                        Id = id,
                        FirstName = ("Admin"),
                        LastName = ("Admin"),
                        EmailAddress = "Admin@gmail.com",
                        Username = "Admin@gmail.com",
                        Password = ("Admin@123"),
                        PhoneNumber = "0648688834",
                        Gender = "Male",
                        DateOfBirth = DateTime.UtcNow,
                        IdNumb = "9701202323000",
                        Balance = 90000,
                        Date = DateTime.UtcNow,
                        HomeAd = "zulu",
                        HomeLang = "zulu",
                        HomeNumber = "0648688834",
                        Maritual = "single",
                        PostalAd = "4001"


                    };
                    //Add The DTO
                    db.Users.Add(userDTO);
                    //Save
                    db.SaveChanges();
                    //Add to UserRolesDTO

                    Fee fee = new Fee()
                    {
                        AdminFee = 1,
                        AfterHourFee = 1,
                        Event_sportFee = 1,
                        Feeding_schemeFee = 2,
                        OneWayDiv = 1,
                        StationaryFee = 1,
                        TwoWayDiv = 1,
                        UniformFee = 1
                    };
                    db.Fees.Add(fee);
                    db.SaveChanges();





                    //SEED DATA//




                    //SEED DATA//


                }
            }
        }
    }
}
