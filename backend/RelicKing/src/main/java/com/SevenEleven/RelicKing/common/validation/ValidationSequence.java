package com.SevenEleven.RelicKing.common.validation;

import jakarta.validation.GroupSequence;

/**
 * validation 우선 순위 설정
 * NotBlankGroup > PatternGroup > EmailGroup
 */
@GroupSequence({
	NotBlankGroup.class,
	PatternGroup.class,
	EmailGroup.class,
})
public interface ValidationSequence {
}