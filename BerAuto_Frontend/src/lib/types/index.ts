// src/lib/types/index.ts
export enum UserType {
	Administrator = 1,
	Worker = 2,
	User = 3,
	System = 4
}

export enum RentStatus {
	Request = 1,
	InProcess = 2,
	Returned = 3,
	Cancelled = 4
}

export enum CarStatus {
	Unavailable = 1,
	Available = 2
}

export interface User {
	id: string;
	name: string;
	email: string;
	address: string;
	phoneNumber: string;
	description: string;
	userType: UserType;
}

export interface Car {
	id: string;
	plateNumber: string;
	type: string;
	odometer: number;
	status: CarStatus;
	categoryId: string;
	description: string;
}

export interface Category {
	id: string;
	name: string;
	dailyRate: number;
	carCount?: number;
}

export interface Rental {
	id: string;
	renterId: string;
	carId: string;
	startDate: string;
	endDate: string;
	status: RentStatus;
	returnDate?: string;
	totalCost?: number;
}