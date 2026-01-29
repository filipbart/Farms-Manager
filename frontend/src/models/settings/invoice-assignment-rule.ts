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
  farmId?: string;
  farmName?: string;
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
  farmId?: string;
}

export interface UpdateInvoiceAssignmentRuleDto {
  name?: string;
  description?: string;
  assignedUserId?: string;
  includeKeywords?: string[];
  excludeKeywords?: string[];
  taxBusinessEntityId?: string;
  farmId?: string;
  isActive?: boolean;
}
