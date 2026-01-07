import ApiUrl from "../common/ApiUrl";
import AxiosWrapper from "../utils/axios/wrapper";
import type {
  InvoiceAssignmentRule,
  CreateInvoiceAssignmentRuleDto,
  UpdateInvoiceAssignmentRuleDto,
} from "../models/settings/invoice-assignment-rule";
import type {
  InvoiceModuleAssignmentRule,
  CreateInvoiceModuleAssignmentRuleDto,
  UpdateInvoiceModuleAssignmentRuleDto,
} from "../models/settings/invoice-module-assignment-rule";

export class SettingsService {
  public static async saveIrzPlusCredentials(data: {
    farmId: string;
    login: string;
    password: string;
  }) {
    return await AxiosWrapper.post(ApiUrl.IrzPlusCredentials, data);
  }

  // Invoice Assignment Rules
  public static async getInvoiceAssignmentRules() {
    return await AxiosWrapper.get<InvoiceAssignmentRule[]>(
      ApiUrl.InvoiceAssignmentRules
    );
  }

  public static async createInvoiceAssignmentRule(
    data: CreateInvoiceAssignmentRuleDto
  ) {
    return await AxiosWrapper.post<string>(ApiUrl.InvoiceAssignmentRules, data);
  }

  public static async updateInvoiceAssignmentRule(
    ruleId: string,
    data: UpdateInvoiceAssignmentRuleDto
  ) {
    return await AxiosWrapper.put(
      `${ApiUrl.InvoiceAssignmentRules}/${ruleId}`,
      data
    );
  }

  public static async deleteInvoiceAssignmentRule(ruleId: string) {
    return await AxiosWrapper.delete(
      `${ApiUrl.InvoiceAssignmentRules}/${ruleId}`
    );
  }

  public static async reorderInvoiceAssignmentRules(orderedRuleIds: string[]) {
    return await AxiosWrapper.post(
      ApiUrl.ReorderInvoiceAssignmentRules,
      orderedRuleIds
    );
  }

  // Invoice Module Assignment Rules
  public static async getInvoiceModuleAssignmentRules() {
    return await AxiosWrapper.get<InvoiceModuleAssignmentRule[]>(
      ApiUrl.InvoiceModuleAssignmentRules
    );
  }

  public static async createInvoiceModuleAssignmentRule(
    data: CreateInvoiceModuleAssignmentRuleDto
  ) {
    return await AxiosWrapper.post<string>(
      ApiUrl.InvoiceModuleAssignmentRules,
      data
    );
  }

  public static async updateInvoiceModuleAssignmentRule(
    ruleId: string,
    data: UpdateInvoiceModuleAssignmentRuleDto
  ) {
    return await AxiosWrapper.put(
      `${ApiUrl.InvoiceModuleAssignmentRules}/${ruleId}`,
      data
    );
  }

  public static async deleteInvoiceModuleAssignmentRule(ruleId: string) {
    return await AxiosWrapper.delete(
      `${ApiUrl.InvoiceModuleAssignmentRules}/${ruleId}`
    );
  }

  public static async reorderInvoiceModuleAssignmentRules(
    orderedRuleIds: string[]
  ) {
    return await AxiosWrapper.post(
      ApiUrl.ReorderInvoiceModuleAssignmentRules,
      orderedRuleIds
    );
  }
}
