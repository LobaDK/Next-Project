import { Injectable } from '@angular/core';
import { Answer } from '../../features/questionnaire/models/answer.model';

interface QuestionnaireSessionState {
    questionnaireId: string;
    userId: string;
    currentQuestionIndex: number;
    answers: Answer[];
    updatedAt: string;
}

type QuestionnaireSessionMap = Record<string, QuestionnaireSessionState>;

@Injectable({
    providedIn: 'root',
})
export class QuestionnaireSessionService {
    private readonly storageKey = 'questionnaire-sessions-v1';

    saveSession(
        questionnaireId: string,
        userId: string,
        currentQuestionIndex: number,
        answers: Answer[]
    ): void {
        if (!questionnaireId || !userId) {
            return;
        }

        if (!this.hasMeaningfulAnswers(answers)) {
            this.removeSession(questionnaireId, userId)
            return;
        }

        const sessions = this.readSessions();
        const key = this.buildSessionKey(questionnaireId, userId);

        sessions[key] = {
            questionnaireId,
            userId,
            currentQuestionIndex,
            answers,
            updatedAt: new Date().toISOString(),
        };

        this.writeSessions(sessions);
    }

    getSession(questionnaireId: string, userId: string): QuestionnaireSessionState | null {
        const sessions = this.readSessions();
        const key = this.buildSessionKey(questionnaireId, userId);
        return sessions[key] ?? null;
    }

    removeSession(questionnaireId: string, userId: string): void {
        const sessions = this.readSessions();
        const key = this.buildSessionKey(questionnaireId, userId);

        if (!(key in sessions)) {
            return;
        }

        delete sessions[key];
        this.writeSessions(sessions);
    }

    clearAllSessions(): void {
        if (typeof localStorage === 'undefined') {
            return;
        }

        try {
            localStorage.removeItem(this.storageKey);
        } catch (error) {
            console.warn('Unable to clear questionnaire sessions from storage.', error);
        }
    }

    clearSessionsForOtherUsers(currentUserId: string): void {
        const sessions = this.readSessions();
        const filtered: QuestionnaireSessionMap = {};

        for (const [key, value] of Object.entries(sessions)) {
            if (value.userId === currentUserId) {
                filtered[key] = value;
            }
        }

        this.writeSessions(filtered);
    }

    private buildSessionKey(questionnaireId: string, userId: string): string {
        return `${userId}::${questionnaireId}`;
    }

    private hasMeaningfulAnswers(answers: Answer[]): boolean {
        return answers.some(answer =>
            !!answer.optionId || !!answer.customAnswer?.trim()
        );
    }

    private readSessions(): QuestionnaireSessionMap {
        if (typeof localStorage === 'undefined') {
            return {};
        }

        let raw: string | null;
        try {
            raw = localStorage.getItem(this.storageKey);
        } catch {
            return {};
        }
        if (!raw) {
            return {};
        }

        try {
            const parsed = JSON.parse(raw) as QuestionnaireSessionMap;
            if (!parsed || typeof parsed !== 'object') {
                return {};
            }

            return parsed;
        } catch {
            return {};
        }
    }

    private writeSessions(sessions: QuestionnaireSessionMap): void {
        if (typeof localStorage === 'undefined') {
            return;
        }

        try {
            localStorage.setItem(this.storageKey, JSON.stringify(sessions));
        } catch (error) {
            if (this.isQuotaExceededError(error)) {
                console.warn('Unable to save questionnaire session: storage quota exceeded.');
                return;
            }

            console.warn('Unable to save questionnaire session due to storage write failure.', error);
        }
    }

    private isQuotaExceededError(error: unknown): boolean {
        if (!(error instanceof DOMException)) {
            return false;
        }

        return (
            error.name === 'QuotaExceededError' ||
            error.name === 'NS_ERROR_DOM_QUOTA_REACHED'
        );
    }
}