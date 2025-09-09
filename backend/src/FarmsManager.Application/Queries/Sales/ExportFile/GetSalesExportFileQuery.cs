using AutoMapper;
using ClosedXML.Excel;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Attributes;
using MediatR;

namespace FarmsManager.Application.Queries.Sales.ExportFile;

public record GetSalesExportFileQuery(GetSalesQueryFilters Filters) : IRequest<Stream>;

public class GetSalesExportFileQueryHandler : IRequestHandler<GetSalesExportFileQuery, Stream>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetSalesExportFileQueryHandler(ISaleRepository saleRepository, IMapper mapper,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<Stream> Handle(GetSalesExportFileQuery request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var accessibleFarmIds = user.AccessibleFarmIds;

        var saleEntites = await _saleRepository.ListAsync(
            new GetAllSalesSpec(request.Filters, false, accessibleFarmIds),
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