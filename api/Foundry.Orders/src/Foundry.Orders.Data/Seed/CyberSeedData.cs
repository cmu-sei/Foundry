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
using Foundry.Orders.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Orders.Data.Seed
{
    public static class CyberSeedData
    {
        public static async Task Seed(OrdersDbContext db)
        {
            var administrator = await db.Profiles.SingleOrDefaultAsync(p => p.GlobalId == "9fd3c38e-58b0-4af1-80d1-1895af91f1f9");

            if (administrator == null)
            {
                administrator = new Profile
                {
                    Name = "Administrator",
                    GlobalId = "9fd3c38e-58b0-4af1-80d1-1895af91f1f9",
                    IsAdministrator = true
                };

                db.Profiles.Add(administrator);

                db.SaveChanges();
            }

            //using (var uow = new UnitOfWork(db, false))
            {

                var assessmentTypes = await AddEntities(db,
                   new string[] { "Knowledge Tests/ Quizzes", "Embedded Observers", "Automated Terrain Sensing", "Specific MOPS/ METS", "After Action Review/ Debriefing", "Other" },
                   (name) => { return new AssessmentType { Name = name }; });

                await AddAudiencesAndAudienceItems(db);
                await AddBranchesAndRanks(db);

                var classifications = await AddEntities(db,
                   new string[] { "UNCLASS", "CONFIDENTIAL", "SECRET", "TOP SECRET", "TS/SCI", "Other" },
                   (name) => { return new Classification { Name = name }; });

                var contentTypes = await AddEntities(db,
                   new string[] { "Scenario Exercise", "Tactical Sandbox", "Guided Hands-on Lab", "Automated Pre-scripted Scenario", "Performance-based Assessment", "Video Lecture", "Video Deomonstration", "Instruction" },
                   (name) => { return new ContentType { Name = name }; });

                var embeddedTeams = await AddEntities(db,
                    new string[] { "Intel Community Provider Products", "Open Source Products", "Tippers" },
                    (name) => { return new EmbeddedTeam { Name = name }; });

                var eventTypes = await AddEntities(db,
                    new string[] { "Scheduled Event - Team Training Exercise", "Scheduled Event - Team Validation Event", "On-Demand - Guided Practice Lab", "On-Demand - Skills Assessment Lab", "On-Demand - Automated Pre-scripted Scenario", "On-Demand - Tactical Sandbox (no scenario)" },
                    (name) =>
                    {
                        var eventType = new EventType { Name = name };
                        if (name.StartsWith("Scheduled Event")) eventType.HasDates = true;
                        if (name.StartsWith("On-Demand")) eventType.HasDuration = true;
                        return eventType;
                    });

                var facilities = await AddEntities(db,
                    new string[] { "Software Engineering Institute" },
                    (name) => { return new Facility { Name = name }; });

                var os = await AddEntities(db,
                    new string[] { "Red Hat Linux", "Windows 2008", "Windows 2012", "Windows 2016", "Windows 7", "Other"},
                    (name) => { return new OperatingSystemType { Name = name }; });

                var producers = await AddEntities(db,
                    new string[] { "CMU Software Engineering Institute" },
                    (name) => { return new Producer { Name = name }; });

                var securityTools = await AddEntities(db,
                    new string[] { "Security Onion", "Rock NSM", "CAPESTACK", "SEIM-ArcSight", "SEIM-Splunk", "CVA-Hunt", "Other" },
                    (name) => { return new SecurityTool { Name = name }; });

                var services = await AddEntities(db,
                    new string[] { "Exchange", "Active Directory", "SharePoint", "HTTP", "SMB", "Chat", "Other" },
                    (name) => { return new Service { Name = name }; });

                var simulators = await AddEntities(db,
                    new string[] { "JWICS", "Weapon Systems", "User Simulation", "Traffic generation", "SCADA/ICS", "Internet (Core Routing)", "TOR", "Bitcoin", "Other" },
                    (name) => { return new Simulator { Name = name }; });

                var support = await AddEntities(db,
                    new string[] { "Live OPFOR", "Automated Injects", "White Cell", "Role Players (Mission Partners, Intel, HHQ, CNDSP, etc.)", "Other" },
                    (name) => { return new Support { Name = name }; });

                var terrains = await AddEntities(db,
                    new string[] { "Specific Malware", "Custom Malware", "C2 Infrastructure in Greyspace", "Legacy Systems", "Other" },
                    (name) => { return new Terrain { Name = name }; });

                var threats = await AddEntities(db,
                    new string[] { "Nation-State", "Hacktivist", "Organized Crime", "Insider(s)", "Other" },
                    (name) => { return new Threat { Name = name }; });


                if (!db.Orders.Any())
                {
                    var facility = facilities.First();

                    await AddOrder(db, administrator, "Jane Doe", "Order 1", "The objective of this order is for testing the order portal.", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla sed luctus nisi, at placerat erat. Maecenas lacinia felis eu pulvinar pulvinar.", DateTime.UtcNow.AddYears(1), db.Classifications.First(), db.Audiences.First(), db.Branches.First(), facility);
                    await AddOrder(db, administrator, "John Doe", "Order 2", "The objective of this order is to create a 500 person exercise.", "Mauris vel sem non tellus interdum mollis non at sapien.", DateTime.UtcNow.AddMonths(3), db.Classifications.Last(), db.Audiences.First(), db.Branches.First(), facility);
                    await AddOrder(db, administrator, "Jane Doe", "Order 3", "The objective of this order is to request custom training for my CPTs.", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla sed luctus nisi, at placerat erat. Maecenas lacinia felis eu pulvinar pulvinar.", DateTime.UtcNow.AddDays(3), db.Classifications.First(), db.Audiences.First(), db.Branches.First(), facility);

                    await AddOrder(db, administrator, "John Doe", "Order 4", "Building A Better Box.", "Maecenas at efficitur odio, et iaculis tortor. Fusce risus ex, scelerisque a fringilla a, fermentum ac purus. Vestibulum sit amet lorem nec nunc malesuada feugiat. ", DateTime.UtcNow.AddDays(45), db.Classifications.First(), db.Audiences.First(), db.Branches.First(), facility, OrderStatus.Submitted);
                    await AddOrder(db, administrator, "Jane Doe", "Order 4", "Learning And Earning.", "Mauris vel sem non tellus interdum mollis non at sapien.", DateTime.UtcNow, db.Classifications.First(), db.Audiences.Last(), db.Branches.First(), facility, OrderStatus.Submitted);
                    await AddOrder(db, administrator, "John Doe", "Order 5", "Clearing The Way.", "Maecenas at efficitur odio, et iaculis tortor. Fusce risus ex, scelerisque a fringilla a, fermentum ac purus. Vestibulum sit amet lorem nec nunc malesuada feugiat. ", DateTime.UtcNow.AddHours(20), db.Classifications.First(), db.Audiences.First(), db.Branches.First(), facility, OrderStatus.InProgress);
                    await AddOrder(db, administrator, "Jane Doe", "Order 5", "Magpie.", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla sed luctus nisi, at placerat erat. Maecenas lacinia felis eu pulvinar pulvinar.", DateTime.UtcNow.AddMonths(10), db.Classifications.First(), db.Audiences.First(), db.Branches.Last(), facility, OrderStatus.InProgress);
                    await AddOrder(db, administrator, "John Doe", "Order 6", "Branding Through Standing.", "Etiam mattis ligula rutrum eros auctor, eget pharetra neque luctus. Phasellus sollicitudin ipsum et risus vestibulum tristique. ", DateTime.UtcNow.AddDays(120), db.Classifications.First(), db.Audiences.Last(), db.Branches.First(), facility, OrderStatus.Complete);
                    await AddOrder(db, administrator, "Jane Doe", "Order 6", "Shifting The Focus.", "Maecenas at efficitur odio, et iaculis tortor. Fusce risus ex, scelerisque a fringilla a, fermentum ac purus. Vestibulum sit amet lorem nec nunc malesuada feugiat. ", DateTime.UtcNow.AddMonths(4), db.Classifications.First(), db.Audiences.First(), db.Branches.First(), facility, OrderStatus.Complete);

                }

                //await uow.CommitTransactionAsync();
            }
        }

        static async Task<IQueryable<TEntity>> AddEntities<TEntity>(OrdersDbContext db, string[] names, Func<string, TEntity> newEntity)
            where TEntity : class, new()
        {
            var dbSet = db.Set<TEntity>();

            if (!dbSet.Any())
            {
                foreach (var name in names)
                {
                    dbSet.Add(newEntity(name));
                }

                db.SaveChanges();
            }

            return db.Set<TEntity>();
        }

        private static async Task AddAudiencesAndAudienceItems(OrdersDbContext db)
        {
            if (!db.Audiences.Any())
            {
                var individual = new Audience { Name = "Individuals (1000/2000)" };
                var team = new Audience { Name = "Collective Teams (3000/4000)" };
                db.Audiences.AddRange(new Audience[] { team, individual });
                db.SaveChanges();

                if (!db.AudienceItems.Any())
                {
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "All Source- Collection Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "All Source- Collection Requirements Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "All- Source Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Authorizing Official/ Designating Representative" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Communications Security (COMSEC) Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Crime Investigator" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Defense Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Defense Forensics Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Defense Incident Responder" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Defense Infrastructure Support Specialist" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Instructional Curriculum Developer" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Instructor" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Intel Planner" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Legal Advisor" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Operator" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Ops Planner" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Policy and Strategy Planner" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Cyber Workforce Developer and Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Data Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Database Administrator" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Enterprise Architect" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Executive Cyber Leadership" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Exploitation Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "IT Investment/ Portfolio Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "IT Program Auditor" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "IT Project Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Information Systems Security Developer" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Information Systems Security Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Knowledge Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Law Enforcement/ Counterintelligence Forensics Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Mission Assessment Specialist" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Multi - Disciplined Language Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Network Operations Specialist" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Partner Integration Planner" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Privacy Officer/ Privacy Compliance Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Product Support Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Program Manager" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Research & amp; Development Specialist" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Secure Software Assessor" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Security Architect" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Security Control Assessor" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Software Developer" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "System Administrator" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "System Testing and Evaluation Specialist" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Systems Developer" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Systems Requirements Planner" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Systems Security Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Target Developer" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Target Network Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Technical Support Specialist" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Threat / Warning Analyst" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = individual.Id, Name = "Vulnerability Assessment Analyst" });
                    db.SaveChanges();

                    db.AudienceItems.Add(new AudienceItem { AudienceId = team.Id, Name = "CPT" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = team.Id, Name = "CMT" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = team.Id, Name = "NMT" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = team.Id, Name = "CST" });
                    db.AudienceItems.Add(new AudienceItem { AudienceId = team.Id, Name = "Other" });
                    db.SaveChanges();
                }
            }
        }

        static async Task AddBranchesAndRanks(OrdersDbContext db)
        {
            var army = db.Branches.SingleOrDefault(x => x.Name == "Army") ?? new Branch { Name = "Army" };
            var navy = db.Branches.SingleOrDefault(x => x.Name == "Navy") ?? new Branch { Name = "Navy" };
            var marineCorp = db.Branches.SingleOrDefault(x => x.Name == "Marine Corps") ?? new Branch { Name = "Marine Corps" };
            var airForce = db.Branches.SingleOrDefault(x => x.Name == "Air Force") ?? new Branch { Name = "Air Force" };
            var coastGuard = db.Branches.SingleOrDefault(x => x.Name == "Coast Guard") ?? new Branch { Name = "Coast Guard" };
            var federalGovernment = db.Branches.SingleOrDefault(x => x.Name == "Federal Government") ?? new Branch { Name = "Federal Government" };
            var other = db.Branches.SingleOrDefault(x => x.Name == "Other") ?? new Branch { Name = "Other" };

            if (!db.Branches.Any())
            {
                db.Branches.AddRange(new Branch[] { army, navy, marineCorp, airForce, coastGuard, federalGovernment, other });
                db.SaveChanges();
            }

            if (!db.Ranks.Any())
            {

                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-1", Name = "Private" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-2", Name = "Private 2" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-3", Name = "Private First Class" });

                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-4", Name = "Specialist" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-4", Name = "Corporal" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-5", Name = "Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-6", Name = "Staff Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-7", Name = "Sergeant First Class	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-8", Name = "Master Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-8", Name = "First Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-9", Name = "Sergeant Major	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-9", Name = "Command Sergeant Major	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "E-9", Name = "Sergeant Major of the Army	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "W-1", Name = "Warrant Officer 1	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "W-2", Name = "Chief Warrant Officer 2	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "W-3", Name = "Chief Warrant Officer 3	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "W-4", Name = "Chief Warrant Officer 4	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "W-5", Name = "Chief Warrant Officer 5	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-1", Name = "Second Lieutenant	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-2", Name = "First Lieutenant	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-3", Name = "Captain	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-4", Name = "Major	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-5", Name = "Lieutenant Colonel	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-6", Name = "Colonel	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-7", Name = "Brigadier General	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-8", Name = "Major General	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-9", Name = "Lieutenant General	" });
                db.Ranks.Add(new Rank { BranchId = army.Id, Grade = "O-10", Name = "General	" });

                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-1", Name = "Seaman Recruit" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-2", Name = "Seaman Apprentice" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-3", Name = "Seaman" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-4", Name = "Petty Officer Third Class	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-5", Name = "Petty Officer Second Class	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-6", Name = "Petty Officer First Class	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-7", Name = "Chief Petty Officer	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-8", Name = "Senior Chief Petty Officer	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-9", Name = "Master Chief Petty Officer	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-9", Name = "Command Master Chief Petty Officer	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "E-9", Name = "Master Chief Petty Officer Of The Navy	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "W-2", Name = "Chief Warrant Officer 2	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "W-3", Name = "Chief Warrant Officer 3	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "W-4", Name = "Chief Warrant Officer 4	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "W-5", Name = "Chief Warrant Officer 5	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-1", Name = "Ensign	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-2", Name = "Lieutenant Junior Grade	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-3", Name = "Lieutenant	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-4", Name = "Lieutenant Commander	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-5", Name = "Commander	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-6", Name = "Captain	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-7", Name = "Rear Admiral Lower Half	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-8", Name = "Rear Admiral	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-9", Name = "Vice Admiral	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-10", Name = "Admiral	" });
                db.Ranks.Add(new Rank { BranchId = navy.Id, Grade = "O-11", Name = "Fleet Admiral	" });

                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-1", Name = "Airman Basic" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-2", Name = "Airman" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-3", Name = "Airman First Class" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-4", Name = "Senior Airman" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-5", Name = "	Staff Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-6", Name = "	Technical Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-7", Name = "	Master Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-8", Name = "	Senior Master Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-9", Name = "	Chief Master Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-9", Name = "	Command Chief Master Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "E-9", Name = "	Chief Master Sergeant Of The Air Force	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-1", Name = "	Second Lieutenant	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-2", Name = "	First Lieutenant	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-3", Name = "	Captain	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-4", Name = "	Major	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-5", Name = "	Lieutenant Colonel	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-6", Name = "	Colonel	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-7", Name = "	Brigadier General	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-8", Name = "	Major General	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-9", Name = "	Lieutenant General	" });
                db.Ranks.Add(new Rank { BranchId = airForce.Id, Grade = "O-10", Name = "General	" });

                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-1", Name = "Private" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-2", Name = "Private First Class" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-3", Name = "Lance Corporal" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-4", Name = "Corporal	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-5", Name = "Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-6", Name = "Staff Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-7", Name = "Gunnery Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-8", Name = "Master Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-8", Name = "First Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-9", Name = "Master Gunnery Sergeant	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-9", Name = "Sergeant Major	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "E-9", Name = "Sergeant Major Of The Marine Corps	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "W-1", Name = "Warrant Officer 1	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "W-2", Name = "Chief Warrant Officer 2	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "W-3", Name = "Chief Warrant Officer 3	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "W-4", Name = "Chief Warrant Officer 4	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "W-5", Name = "Chief Warrant Officer 5	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-1", Name = "Second Lieutenant	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-2", Name = "First Lieutenant	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-3", Name = "Captain	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-4", Name = "Major	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-5", Name = "Lieutenant Colonel	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-6", Name = "Colonel	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-7", Name = "Brigadier General	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-8", Name = "Major General	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-9", Name = "Lieutenant General	" });
                db.Ranks.Add(new Rank { BranchId = marineCorp.Id, Grade = "O-10", Name = "General" });

                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-1", Name = "Seaman Recruit" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-2", Name = "Seaman Apprentice" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-2", Name = "Fireman Apprentice" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-2", Name = "Airman Apprentice" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-3", Name = "Seaman" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-3", Name = "Fireman" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-3", Name = "Airman" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-4", Name = "Petty Officer Third Class	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-5", Name = "Petty Officer Second Class	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-6", Name = "Petty Officer First Class	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-7", Name = "Chief Petty Officer	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-8", Name = "Senior Chief Petty Officer	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-9", Name = "Master Chief Petty Officer	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-9", Name = "Command Master Chief Petty Officer	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "E-9", Name = "Master Chief Petty Officer Of The Coast Guard	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "W-2", Name = "Chief Warrant Officer 2	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "W-3", Name = "Chief Warrant Officer 3	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "W-4", Name = "Chief Warrant Officer 4	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-1", Name = "Ensign	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-2", Name = "Lieutenant Junior Grade	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-3", Name = "Lieutenant	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-4", Name = "Lieutenant Commander	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-5", Name = "Commander	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-6", Name = "Captain	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-7", Name = "Rear Admiral Lower Half	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-8", Name = "Rear Admiral	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-9", Name = "Vice Admiral	" });
                db.Ranks.Add(new Rank { BranchId = coastGuard.Id, Grade = "O-10", Name = "Admiral	" });

                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-1", Name = "GS-1" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-2", Name = "GS-2" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-3", Name = "GS-3" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-4", Name = "GS-4" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-5", Name = "GS-5" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-6", Name = "GS-6" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-7", Name = "GS-7" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-8", Name = "GS-8" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-9", Name = "GS-9" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-10", Name = "GS-10" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-11", Name = "GS-11" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-12", Name = "GS-12" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-13", Name = "GS-13" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-14", Name = "GS-14" });
                db.Ranks.Add(new Rank { BranchId = federalGovernment.Id, Grade = "GS-15", Name = "GS-15" });

                db.SaveChanges();
            }
        }

        public static async Task<Order> AddOrder(OrdersDbContext db, Profile user, string requestor, string name, string objectives, string description, DateTime? due, Classification classification, Audience audience, Branch branch, Facility facility, OrderStatus status = OrderStatus.Draft)
        {
            var order = new Order
            {
                Due = due,
                Description = description,
                Requestor = requestor,
                Objectives = objectives,
                ClassificationId = classification.Id,
                AudienceId = audience.Id,
                BranchId = branch.Id,
                FacilityId = facility.Id,
                Status = status,
                Created = DateTime.UtcNow,
                CreatedById = user.Id
            };

            db.Orders.Add(order);

            db.SaveChanges();

            return order;
        }
    }
}

