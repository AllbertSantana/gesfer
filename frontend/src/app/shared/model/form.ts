import { ValidatorFn } from "@angular/forms";

export enum ValidatorType {
	min = "min",
	max = "max",
	required = "required",
	requiredTrue = "requiredTrue",
	email = "email",
	minLength = "minLength",
	maxLength = "maxLength",
	pattern = "pattern",
	nullValidator = "nullValidator",
	compose = "compose",
	composeAsync = "composeAsync",
}

export enum FormType {
	filter = "filter",
	add = "add",
	edit = "edit",
}

export enum InputType {
	textspan = "textspan",
	select = "select"
}

export interface SelectInputOption {
	label: string;
	value: string | number;
}

export interface InputValidator {
	type: ValidatorType;
	message: string;
	validator: ValidatorFn;
}

export interface InputControlConfig {
	name: string;
	value: string | number | boolean;
	isDisabled: boolean;
	usedAt: FormType[];
	validators?: InputValidator[];
}

export interface InputViewConfig {
	label: string;
	type: InputType;
	options?: SelectInputOption[];
	mask?: (string | RegExp)[];
}

export interface InputConfig {
	control: InputControlConfig;
	view: InputViewConfig;
}