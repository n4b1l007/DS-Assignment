using Ds.Application.Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DsApi
{
    public class PurchaseOrderDocument : IDocument
    {
        private PurchaseOrderDto _purchaseOrder;
        private static readonly Color PrimaryColor = Colors.Blue.Medium;

        public PurchaseOrderDocument(PurchaseOrderDto purchaseOrder)
        {
            _purchaseOrder = purchaseOrder;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("PURCHASE ORDER").FontSize(20).SemiBold().FontColor(PrimaryColor);
                    column.Item().Text($"Order Number: {_purchaseOrder.OrderNumber}").FontSize(12).Bold();
                    column.Item().Text($"Reference ID: {_purchaseOrder.ReferenceId}");
                });

                //row.ConstantItem(100).Height(100).Placeholder();

                row.RelativeItem().Column(column =>
                {
                    column.Item().BorderBottom(1).PaddingBottom(5).Text("Supplier Information").SemiBold();
                    column.Item().Text(_purchaseOrder.SupplierName);
                    column.Item().Text($"Supplier ID: {_purchaseOrder.SupplierId}");
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(20);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new InfoSection("Order Date", _purchaseOrder.OrderDate.ToShortDateString()));
                    row.RelativeItem().Component(new InfoSection("Expected Date", _purchaseOrder.ExpectedDate.ToShortDateString()));
                });

                column.Item().Element(ComposeTable);

                column.Item().PaddingTop(25).Element(ComposeTotal);

                if (!string.IsNullOrEmpty(_purchaseOrder.Remark))
                {
                    column.Item().PaddingTop(25).Component(new InfoSection("Remarks", _purchaseOrder.Remark));
                }
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Item Name");
                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                    header.Cell().Element(CellStyle).AlignRight().Text("Rate");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var (item, index) in _purchaseOrder.OrderDetails.Select((item, index) => (item, index)))
                {
                    table.Cell().Element(CellStyle).Text($"{index + 1}");
                    table.Cell().Element(CellStyle).Text(item.ItemName);
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Rate:C2}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity * item.Rate:C2}");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }

        private void ComposeTotal(IContainer container)
        {
            container.DefaultTextStyle(x => x.SemiBold()).Row(row =>
            {
                row.RelativeItem().Text("Total:");
                row.ConstantItem(100).AlignRight().Text($"{_purchaseOrder.OrderDetails.Sum(x => x.Quantity * x.Rate):C2}");
            });
        }
    }

    public class InfoSection : IComponent
    {
        private string Title { get; }
        private string Value { get; }

        public InfoSection(string title, string value)
        {
            Title = title;
            Value = value;
        }

        public void Compose(IContainer container)
        {
            container.ShowEntire().Column(column =>
            {
                column.Spacing(2);
                column.Item().Text(Title).SemiBold().FontColor(Colors.Grey.Medium);
                column.Item().Text(Value);
            });
        }
    }
}
