export function getFileTypeFromUrl(url: string): "pdf" | "image" | "unknown" {
  const urlWithoutQuery = url.split("?")[0].toLowerCase();

  if (urlWithoutQuery.endsWith(".pdf")) {
    return "pdf";
  }

  if (urlWithoutQuery.match(/\.(jpeg|jpg|png)$/)) {
    return "image";
  }

  return "unknown";
}

export function getMimeTypeFromUrl(url: string): string {
  const urlWithoutQuery = url.split("?")[0].toLowerCase();

  if (urlWithoutQuery.endsWith(".pdf")) {
    return "application/pdf";
  }

  if (urlWithoutQuery.match(/\.(jpeg|jpg)$/)) {
    return "image/jpeg";
  }

  if (urlWithoutQuery.endsWith(".png")) {
    return "image/png";
  }

  return "application/octet-stream";
}
