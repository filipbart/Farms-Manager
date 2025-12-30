using System.IO.Compression;
using System.Text;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoicePdf;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.GetInvoicesZip;

/// <summary>
/// Query do pobierania plików faktur jako ZIP
/// </summary>
public record GetInvoicesZipQuery(List<Guid> InvoiceIds) : IRequest<InvoicesZipResult>;

public class InvoicesZipResult
{
    public byte[] ZipData { get; set; }
    public string FileName { get; set; }
    public int FileCount { get; set; }
}

public class GetInvoicesZipQueryHandler : IRequestHandler<GetInvoicesZipQuery, InvoicesZipResult>
{
    private readonly IMediator _mediator;
    private readonly IUserDataResolver _userDataResolver;

    public GetInvoicesZipQueryHandler(IMediator mediator, IUserDataResolver userDataResolver)
    {
        _mediator = mediator;
        _userDataResolver = userDataResolver;
    }

    public async Task<InvoicesZipResult> Handle(GetInvoicesZipQuery request, CancellationToken cancellationToken)
    {
        _ = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        if (request.InvoiceIds == null || request.InvoiceIds.Count == 0)
        {
            throw DomainException.BadRequest("Nie wybrano żadnych faktur.");
        }

        using var memoryStream = new MemoryStream();
        var fileCount = 0;
        
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var invoiceId in request.InvoiceIds)
            {
                try
                {
                    var pdfResult = await _mediator.Send(new GetKSeFInvoicePdfQuery(invoiceId), cancellationToken);
                    
                    var entry = archive.CreateEntry(pdfResult.FileName, CompressionLevel.Optimal);
                    await using var entryStream = entry.Open();
                    await entryStream.WriteAsync(pdfResult.Data, cancellationToken);
                    fileCount++;
                }
                catch
                {
                    // Pomiń faktury, które nie mogą być wygenerowane jako PDF
                }
            }
        }

        memoryStream.Position = 0;
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        
        return new InvoicesZipResult
        {
            ZipData = memoryStream.ToArray(),
            FileName = $"Faktury_{timestamp}.zip",
            FileCount = fileCount
        };
    }
}
