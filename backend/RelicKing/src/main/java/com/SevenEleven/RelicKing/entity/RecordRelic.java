package com.SevenEleven.RelicKing.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Embeddable
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@ToString
public class RecordRelic {

	@Column(nullable = false)
	private int relicNo;

	@Column(nullable = false)
	private int level;

	@Column(nullable = false)
	private int slot;
}
