import { useState, useEffect, useRef, useCallback } from "react";
import {
  AccountingService,
  type InvoiceAttachment,
} from "../../../../services/accounting-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { toast } from "react-toastify";

interface UseInvoiceAttachmentsProps {
  invoiceId: string | undefined;
}

interface UseInvoiceAttachmentsReturn {
  attachments: InvoiceAttachment[];
  attachmentsLoading: boolean;
  uploadingAttachment: boolean;
  fileInputRef: React.RefObject<HTMLInputElement | null>;
  attachmentToDelete: InvoiceAttachment | null;
  setAttachmentToDelete: (attachment: InvoiceAttachment | null) => void;
  handleAttachmentUpload: (
    event: React.ChangeEvent<HTMLInputElement>,
  ) => Promise<void>;
  handleAttachmentDownload: (attachment: InvoiceAttachment) => Promise<void>;
  handleAttachmentDelete: (attachmentId: string) => void;
  confirmAttachmentDelete: () => Promise<void>;
}

export const useInvoiceAttachments = ({
  invoiceId,
}: UseInvoiceAttachmentsProps): UseInvoiceAttachmentsReturn => {
  const [attachments, setAttachments] = useState<InvoiceAttachment[]>([]);
  const [attachmentsLoading, setAttachmentsLoading] = useState(false);
  const [uploadingAttachment, setUploadingAttachment] = useState(false);
  const [attachmentToDelete, setAttachmentToDelete] =
    useState<InvoiceAttachment | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Fetch attachments
  const fetchAttachments = useCallback(async () => {
    if (!invoiceId) return;
    setAttachmentsLoading(true);
    try {
      const response = await AccountingService.getAttachments(invoiceId);
      if (response.success && response.responseData) {
        setAttachments(response.responseData);
      }
    } catch {
      // Ignore errors
    } finally {
      setAttachmentsLoading(false);
    }
  }, [invoiceId]);

  useEffect(() => {
    fetchAttachments();
  }, [fetchAttachments]);

  // Handle attachment upload
  const handleAttachmentUpload = async (
    event: React.ChangeEvent<HTMLInputElement>,
  ) => {
    const file = event.target.files?.[0];
    if (!file || !invoiceId) return;

    setUploadingAttachment(true);
    try {
      await handleApiResponse(
        () => AccountingService.uploadAttachment(invoiceId, file),
        (data) => {
          if (data.responseData) {
            setAttachments((prev) => [data.responseData!, ...prev]);
            toast.success("Załącznik został dodany");
          }
        },
        undefined,
        "Błąd podczas dodawania załącznika",
      );
    } finally {
      setUploadingAttachment(false);
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
    }
  };

  // Handle attachment download
  const handleAttachmentDownload = async (attachment: InvoiceAttachment) => {
    if (!invoiceId) return;
    try {
      const blob = await AccountingService.downloadAttachment(
        invoiceId,
        attachment.id,
      );
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = attachment.fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch {
      toast.error("Błąd podczas pobierania załącznika");
    }
  };

  // Handle attachment delete
  const handleAttachmentDelete = (attachmentId: string) => {
    const target = attachments.find((a) => a.id === attachmentId);
    setAttachmentToDelete(target ?? null);
  };

  const confirmAttachmentDelete = async () => {
    if (!invoiceId || !attachmentToDelete) return;

    try {
      await handleApiResponse(
        () =>
          AccountingService.deleteAttachment(invoiceId, attachmentToDelete.id),
        () => {
          setAttachments((prev) =>
            prev.filter((a) => a.id !== attachmentToDelete.id),
          );
          toast.success("Załącznik został usunięty");
        },
        undefined,
        "Błąd podczas usuwania załącznika",
      );
    } finally {
      setAttachmentToDelete(null);
    }
  };

  return {
    attachments,
    attachmentsLoading,
    uploadingAttachment,
    fileInputRef,
    attachmentToDelete,
    setAttachmentToDelete,
    handleAttachmentUpload,
    handleAttachmentDownload,
    handleAttachmentDelete,
    confirmAttachmentDelete,
  };
};
