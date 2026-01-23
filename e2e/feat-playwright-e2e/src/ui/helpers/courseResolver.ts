import { courseData } from '../config/courseData';
import { getEnv } from './env';

type CourseType = 'degree' | 'ncs' | 'apprenticeship';

export function getCourseId(type: CourseType): string {
    const env = getEnv('FEAT_ENV') as keyof typeof courseData;

    const courseId = courseData[env]?.[type];

    if (!courseId) {
        throw new Error(
            `Course ID not configured for type "${type}" in environment "${env}"`
        );
    }

    return courseId;
}
