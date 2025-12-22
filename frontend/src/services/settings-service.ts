import ApiUrl from "../common/ApiUrl";
import AxiosWrapper from "../utils/axios/wrapper";
import type {
  InvoiceAssignmentRule,
  CreateInvoiceAssignmentRuleDto,
  UpdateInvoiceAssignmentRuleDto,
} from "../models/settings/invoice-assignment-rule";

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
}
