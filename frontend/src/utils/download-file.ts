import axios from "axios";
import qs from "qs";
import { toast } from "react-toastify";

interface DownloadFileParams {
  url: string;
  params?: Record<string, any>;
  defaultFilename: string;
  setLoading: (loading: boolean) => void;
  successMessage?: string;
  errorMessage: string;
  fileExtension?: string;
}

export const downloadFile = async ({
  url,
  params,
  defaultFilename,
  setLoading,
  successMessage,
  errorMessage,
  fileExtension = "pdf",
}: DownloadFileParams) => {
  try {
    setLoading(true);

    const response = await axios.get(url, {
      responseType: "blob",
      params,
      paramsSerializer: (params) =>
        qs.stringify(params, { arrayFormat: "repeat" }),
    });

    const blob = new Blob([response.data]);

    if (blob.size === 0) {
      toast.warning("Brak danych w pliku");
      return;
    }

    const disposition = response.headers["content-disposition"];
    let filename = `${defaultFilename}_${new Date().toISOString()}.${fileExtension}`;

    if (disposition && disposition.includes("filename=")) {
      const fileNameMatch = disposition.match(
        /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/
      );
      if (fileNameMatch?.length >= 2) {
        filename = fileNameMatch[1].replace(/['"]/g, "");
      }
    }

    const blobUrl = window.URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = blobUrl;
    link.setAttribute("download", filename);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(blobUrl);

    if (successMessage) {
      toast.success(successMessage);
    }
  } catch {
    toast.error(errorMessage);
  } finally {
    setLoading(false);
  }
};
