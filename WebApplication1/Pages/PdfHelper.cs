using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using System.IO;

public static class PdfHelper
{
    public static byte[] GenerateShipmentPdf(Shipment shipment)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Content()
                    .Column(col =>
                    {
                        // Title
                        // Title
                        col.Item().Element(container =>
                            container.Text("Builty Details")
                                .FontSize(22)
                                .Bold()
                                .AlignCenter()
                        );



                        // Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(180); // Field
                                columns.RelativeColumn();    // Value
                                columns.RelativeColumn();    // Optional third column
                            });

                            // Helper for cell style
                            static IContainer CellStyle(IContainer container) =>
                                container.Border(1)
                                         .BorderColor(Colors.Grey.Lighten1)
                                         .PaddingVertical(6)
                                         .PaddingHorizontal(8);

                            // Helper for header cell style
                            static IContainer HeaderStyle(IContainer container) =>
                                container.Background(Colors.Grey.Lighten3)
                                         .Border(1)
                                         .BorderColor(Colors.Grey.Darken1)
                                         .PaddingVertical(8)
                                         .PaddingHorizontal(8);

                            // AddRow for 2 columns
                            void AddRow(string field, string value)
                            {
                                table.Cell().Element(CellStyle).Text(field).SemiBold();
                                table.Cell().Element(CellStyle).Text(value ?? "");
                                table.Cell().Element(CellStyle); // Empty cell for alignment
                            }

                            // AddRow for 3 columns
                            void AddRowWithExtra(string field, string value, string extra)
                            {
                                table.Cell().Element(CellStyle).Text(field).SemiBold();
                                table.Cell().Element(CellStyle).Text(value ?? "");
                                table.Cell().Element(CellStyle).Text(extra ?? "");
                            }

                            // Row 1: Shipment Date, Builty Number
                            AddRow("Shipment Date", shipment.ShipmentDateTime.ToString("yyyy-MM-dd"));
                            AddRow("Builty Number", shipment.Id.ToString());

                            // Row 2: Sender Name, Receiver Name
                            AddRow("Sender Name", shipment.Sender?.Name);
                            AddRow("Receiver Name", shipment.Receiver?.Name);

                            // Row 3: Booking Office, Destination City
                            AddRow("Booking Office", shipment.BookingOffice);
                            AddRow("Destination City", shipment.City);

                            // Row 4: Number of Items, Total Weight
                            AddRow("Number of Items", shipment.NumberOfItems.ToString());
                            AddRow("Total Weight", shipment.TotalWeight.ToString());

                            // Row 5: Description, Amount
                            AddRow("Description", shipment.Description);
                            AddRow("Amount", shipment.Price.ToString("C"));

                            // Row 6: Payment Status, Payment Received
                            AddRow("Payment Status", shipment.PaymentStatus);
                            AddRow("Payment Received", shipment.ReceivePayment ? "Yes" : "No");
                        });
                    });
            });
        }).GeneratePdf();
    }


}
