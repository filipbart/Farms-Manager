using AutoMapper;
using ClosedXML.Excel;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Shared.Attributes;
using MediatR;

namespace FarmsManager.Application.Queries.Sales.ExportFile;

public record GetSalesExportFileQuery(GetSalesQueryFilters Filters) : IRequest<Stream>;

public class GetSalesExportFileQueryHandler : IRequestHandler<GetSalesExportFileQuery, Stream>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSalesExportFileQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<Stream> Handle(GetSalesExportFileQuery request, CancellationToken cancellationToken)
    {
        var saleEntites = await _saleRepository.ListAsync(new GetAllSalesSpec(request.Filters, false),
            cancellationToken);

        if (saleEntites.Count == 0)
        {
            return null;
        }

        var sales = _mapper.Map<List<SalesExportFileModel>>(saleEntites);

        using var xlWorkBook = new XLWorkbook();
        var ws = xlWorkBook.Worksheets.Add("Sprzedaże");

        var columns = GetColumns();
        var props = typeof(SalesExportFileModel).GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(ExcelAttribute)))
            .ToList();


        for (var col = 0; col < columns.Count; col++)
        {
            var colDef = columns[col];
            ws.Cell(1, col + 1).Value = colDef.ColumnName;
            ws.Cell(1, col + 1).Style.Font.Bold = true;
            ws.Cell(1, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            if (!string.IsNullOrEmpty(colDef.CellFormat))
            {
                ws.Column(col + 1).Style.NumberFormat.Format = colDef.IsCurrency ? "#,##0.00 zł" : colDef.CellFormat;
            }
        }

        for (var row = 0; row < sales.Count; row++)
        {
            var item = sales[row];

            for (var col = 0; col < props.Count; col++)
            {
                var prop = props[col];
                var value = prop.GetValue(item);
                var attr = prop.GetCustomAttributes(typeof(ExcelAttribute), false).FirstOrDefault() as ExcelAttribute;

                var cell = ws.Cell(row + 2, col + 1);

                if (attr is { IsList: true } && value is IEnumerable<SaleOtherExtrasFileModel> list)
                {
                    var formatted = string.Join(", ",
                        list.Select(x => $"{x.Name}: {x.Value:0.00} zł"));

                    cell.Value = formatted;
                    cell.Style.Alignment.WrapText = true;
                }
                else
                {
                    cell.Value = ConvertToCellValue(value);
                }
            }
        }

        ws.Columns().AdjustToContents();

        var stream = new MemoryStream();
        xlWorkBook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private static List<ExcelAttribute> GetColumns()
    {
        return typeof(SalesExportFileModel)
            .GetProperties()
            .Where(t => t.GetCustomAttributes(typeof(ExcelAttribute), false).Length != 0)
            .Select(t => t.GetCustomAttributes(typeof(ExcelAttribute), false)[0] as ExcelAttribute)
            .ToList();
    }

    private static XLCellValue ConvertToCellValue(object value)
    {
        if (value == null)
            return string.Empty;

        return value switch
        {
            DateOnly dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            DateTime dt => dt,
            decimal dec => dec,
            double d => d,
            float f => f,
            int i => i,
            long l => l,
            bool b => b,
            _ => value.ToString()
        };
    }
}