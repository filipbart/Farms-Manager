export interface InvoiceAssignmentRule {
  id: string;
  name: string;
  description?: string;
  priority: number;
  assignedUserId: string;
  assignedUserName: string;
  includeKeywords: string[];
  excludeKeywords: string[];
  taxBusinessEntityId?: string;
  taxBusinessEntityName?: string;
  farmIds: string[];
  farmNames: Record<string, string>;
  isActive: boolean;
  dateCreatedUtc: string;
}

export interface CreateInvoiceAssignmentRuleDto {
  name: string;
  description?: string;
  assignedUserId: string;
  includeKeywords: string[];
  excludeKeywords: string[];
  taxBusinessEntityId?: string;
  farmIds: string[];
}

export interface UpdateInvoiceAssignmentRuleDto {
  name?: string;
  description?: string;
  assignedUserId?: string;
  includeKeywords?: string[];
  excludeKeywords?: string[];
  taxBusinessEntityId?: string;
  farmIds?: string[];
  isActive?: boolean;
}
