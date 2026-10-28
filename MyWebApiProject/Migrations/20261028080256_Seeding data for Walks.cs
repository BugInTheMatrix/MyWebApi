using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyWebApiProject.Migrations
{
    /// <inheritdoc />
    public partial class SeedingdataforWalks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Walks",
                columns: new[] { "Id", "Description", "DifficultyId", "LengthInKm", "Name", "RegionId", "WalkImageUrl" },
                values: new object[,]
                {
                    { new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a51"), "A short climb to the top of Auckland's highest volcanic cone with panoramic city views.", new Guid("54466f17-02af-48e7-8ed3-5a4a8bfacf6f"), 2.5, "Mount Eden Summit Walk", new Guid("f7248fc3-2585-4efb-8d1d-1c555f4087f6"), null },
                    { new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a52"), "Coastal track past sand dunes and beaches to the northern tip of New Zealand.", new Guid("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"), 16.0, "Cape Reinga Coastal Walkway", new Guid("6884f7d7-ad1f-4101-8df3-7a6fa7387d81"), null },
                    { new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a53"), "Flat loop around the base of Mauao with ocean views and rock pools.", new Guid("54466f17-02af-48e7-8ed3-5a4a8bfacf6f"), 3.3999999999999999, "Mount Maunganui Base Track", new Guid("14ceba71-4b51-4777-9b17-46602cf66153"), null },
                    { new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a54"), "Town belt trails up to the Mount Victoria lookout over Wellington harbour.", new Guid("ea294873-7a8c-4c0f-bfa7-a2eb492cbf8c"), 4.2000000000000002, "Mount Victoria Lookout Walk", new Guid("cfa06ed2-bf65-4b65-93ed-c9d286ddb0de"), null },
                    { new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a55"), "Multi-day Great Walk along golden beaches and granite headlands.", new Guid("f808ddcd-b5e5-4d80-b732-1ca523e48434"), 60.0, "Abel Tasman Coast Track", new Guid("906cb139-415a-4bbb-a174-1a1faf9fb1f6"), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Walks",
                keyColumn: "Id",
                keyValue: new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a51"));

            migrationBuilder.DeleteData(
                table: "Walks",
                keyColumn: "Id",
                keyValue: new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a52"));

            migrationBuilder.DeleteData(
                table: "Walks",
                keyColumn: "Id",
                keyValue: new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a53"));

            migrationBuilder.DeleteData(
                table: "Walks",
                keyColumn: "Id",
                keyValue: new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a54"));

            migrationBuilder.DeleteData(
                table: "Walks",
                keyColumn: "Id",
                keyValue: new Guid("a1c3e5f7-1b2d-4e6f-8a9b-0c1d2e3f4a55"));
        }
    }
}
