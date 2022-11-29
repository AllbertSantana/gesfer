export enum MessageColor {
    primary = 'primary',
    accent = 'accent',
    warn = 'warn',
}

export enum MessageIcon {
    error = 'error',
    warning = 'warning',
    success = 'check_circle',
    info = 'info',
}

export interface MessageType {
    color?: MessageColor;
    icon: MessageIcon;
}

export class MessageTypes {
    static readonly error: MessageType = { color: MessageColor.warn, icon: MessageIcon.error };
    static readonly success: MessageType = { icon: MessageIcon.success };
}

export interface Message {
    title: string;
    type: MessageType;
    subtitle?: string;
    errors?: string[];
}