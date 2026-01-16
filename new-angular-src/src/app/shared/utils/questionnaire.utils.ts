export class QuestionnaireUtils {
  static truncateText(text: string, maxLength: number = 100): string {
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength) + '...';
  }

  static getUserName(userId: number, users: any[]): string {
    return users.find(u => u.id === userId)?.name || `User ${userId}`;
  }

  static getQuestionById(questionId: number, templates: any[]): any {
    for (const template of templates) {
      const question = template.questions.find((q: any) => q.id === questionId);
      if (question) return question;
    }
    return undefined;
  }

  static getTimestampDisplayText(timestamp: Date): string {
    return timestamp.toLocaleDateString() + ' ' + timestamp.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
  }

  static generateColors(): string[] {
    return [
      'rgb(255, 99, 132)',   // Red
      'rgb(54, 162, 235)',   // Blue
      'rgb(255, 205, 86)',   // Yellow
      'rgb(75, 192, 192)',   // Teal
      'rgb(153, 102, 255)',  // Purple
      'rgb(255, 159, 64)'    // Orange
    ];
  }
}