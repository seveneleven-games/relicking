package com.SevenEleven.RelicKing.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@ToString
public class MemberRelic {
	@Id
	@Column(name = "member_relic_id")
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int memberRelicId;

	@ManyToOne
	@JoinColumn(name = "member_id", referencedColumnName = "member_id")
	private Member member;

	@Builder.Default
	@Column(nullable = false)
	private int relicNo = 0;

	@Builder.Default
	@Column(nullable = false)
	private int level = 1;

	@Builder.Default
	@Column(nullable = false)
	private int exp = 0;

	@Builder.Default
	@Column(nullable = false)
	private int slot = 0;
}
